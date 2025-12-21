using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

[System.Serializable]
public class HandData
{
    public float[] thumb;
    public float[] index;
    public float pinch;
    public bool click;
}

public class Moving : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    float xRotation = 0f;

    // --- Footstep sound ---
    public float stepInterval = 0.5f; // thời gian giữa 2 bước
    private float stepTimer = 0f;

    private ChuckSubInstance chuck; // tham chiếu đến Chuck instance
    private bool isOnWater = false;
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    public float handRotateSensitivity = 60f;
    public float zoomSpeed = 10f;

    public float minFov = 30f;
    public float maxFov = 90f;

    private Vector2 lastHandPos;
    private bool hasLastHandPos = false;


    void Start()
    {
        udpClient = new UdpClient(65433); // Cùng port với Python
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        Cursor.lockState = CursorLockMode.None;
        chuck = GetComponent<ChuckSubInstance>();
        if (chuck == null)
        {
            Debug.LogWarning("Thiếu ChuckSubInstance trên Player — thêm component này vào object!");
        }
    }

    void Update()
    {
        if (udpClient.Available > 0)
        {
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            string json = Encoding.UTF8.GetString(data);
            HandData handData = JsonUtility.FromJson<HandData>(json);

            if (handData != null)
            {
                HandleHandRotation(handData);
                HandleHandZoom(handData);
            }
        }

        // --- Xoay bằng chuột ---
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // --- Di chuyển ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.Translate(move * speed * Time.deltaTime, Space.World);

        bool isMoving = move.magnitude > 0.1f;
        Vector3 p = transform.position;
        float distSq = p.x * p.x + p.z * p.z;

        isOnWater = distSq >= 18f * 18f && distSq <= 25f * 25f;


        // --- Phát tiếng bước chân bằng Chuck ---
        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                stepTimer = 0f;
                if (isOnWater)
                    PlayWaterFootstepSound();
                else
                    PlayFootstepSound();

            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void HandleHandRotation(HandData handData)
    {
        if (handData.pinch > 0.1)
        {
            Vector2 thumb = new Vector2(handData.thumb[0], handData.thumb[1]);
            Vector2 index = new Vector2(handData.index[0], handData.index[1]);
            Vector2 handPos = (thumb + index) * 0.5f;

            if (hasLastHandPos)
            {
                Vector2 delta = handPos - lastHandPos;

                float rotX = -delta.y * handRotateSensitivity;
                float rotY = delta.x * handRotateSensitivity;

                xRotation -= rotX;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * rotY);
            }

            lastHandPos = handPos;
            hasLastHandPos = true;
        }
        else
        {
            hasLastHandPos = false;
        }
    }

    void HandleHandZoom(HandData handData)
    {
        Camera cam = playerCamera.GetComponent<Camera>();

        float pinch = Mathf.Clamp(handData.pinch, 0.05f, 0.35f);

        float targetFov = Mathf.Lerp(
            minFov,
            maxFov,
            Mathf.InverseLerp(0.05f, 0.35f, pinch)
        );

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFov,
            Time.deltaTime * zoomSpeed
        );
    }

    void OnDestroy()
    {
        udpClient.Close();
    }


    void PlayFootstepSound()
    {
        if (chuck == null) return;

        chuck.RunCode(@"
        // Tạo 1 tiếng bước chân nhẹ êm
        Noise n => LPF f => ADSR e => JCRev reverb => dac;

        0.1 => reverb.mix;            // reverb nhẹ
        Std.rand2f(300, 700) => f.freq; // tần số lọc thấp
        0.45 => n.gain;               // âm lượng cực nhẹ

        e.set(0.01, 0.05, 0.0, 0.1);  // attack, decay, sustain, release
        Std.rand2(1,2) => int soatCount;

        for(0 => int i; i < soatCount; i++)
        {
            Std.rand2f(0.35,0.55) => e.gain; // random volume cho tự nhiên
            e.keyOn();
            Std.rand2f(0.06,0.12)::second => now; // duration
            e.keyOff();

            Std.rand2f(0.2,0.5)::second => now; // khoảng nghỉ giữa các soạt
        }
    ");
    }

    void PlayWaterFootstepSound()
    {
        if (chuck == null) return;

        chuck.RunCode(@"
        Noise n => LPF lpf => ADSR env => JCRev r => Gain g => dac;
        Std.rand2f(180, 320) => lpf.freq;
        0.25 => r.mix;
        0.18 => g.gain;
        env.set(25::ms, 160::ms, 0.0, 220::ms);

        Noise s => HPF hpf => ADSR e2 => Gain g2 => dac;
        Std.rand2f(2200, 3200) => hpf.freq;
        0.04 => g2.gain;
        e2.set(5::ms, 40::ms, 0, 60::ms);

        env.keyOn();
        90::ms => now;

        e2.keyOn();
        50::ms => now;
        e2.keyOff();

        env.keyOff();
    ");
    }

}

