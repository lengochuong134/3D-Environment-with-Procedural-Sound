using UnityEngine;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        chuck = GetComponent<ChuckSubInstance>();
        if (chuck == null)
        {
            Debug.LogWarning("Thiếu ChuckSubInstance trên Player — thêm component này vào object!");
        }
    }

    void Update()
    {
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

        // --- Phát tiếng bước chân bằng Chuck ---
        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                stepTimer = 0f;
                PlayFootstepSound();
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstepSound()
{
    if (chuck == null) return;

    chuck.RunCode(@"
        // Tạo 1 tiếng bước chân nhẹ êm
        Noise n => LPF f => ADSR e => JCRev reverb => dac;

        0.1 => reverb.mix;            // reverb nhẹ
        Std.rand2f(300, 700) => f.freq; // tần số lọc thấp
        0.25 => n.gain;               // âm lượng cực nhẹ

        e.set(0.01, 0.05, 0.0, 0.1);  // attack, decay, sustain, release
        Std.rand2(1,2) => int soatCount;

        for(0 => int i; i < soatCount; i++)
        {
            Std.rand2f(0.25,0.35) => e.gain; // random volume cho tự nhiên
            e.keyOn();
            Std.rand2f(0.06,0.12)::second => now; // duration
            e.keyOff();

            Std.rand2f(0.2,0.5)::second => now; // khoảng nghỉ giữa các soạt
        }
    ");
}




}
