using UnityEngine;

public class Moving : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera; // gắn Main Camera ở đây

    float xRotation = 0f;

    void Start()
    {
        // Cho chuột tự do ban đầu
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // ----- Chuột xoay khi giữ nút trái -----
        if (Input.GetMouseButton(1)) // 0 = chuột trái, 1 = chuột phải, 2 = giữa
        {
            Cursor.lockState = CursorLockMode.Locked; // khóa chuột khi giữ
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // Thả chuột → unlock để rê chuột tự do
            Cursor.lockState = CursorLockMode.None;
        }

        // ----- Di chuyển phím WASD -----
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
