using UnityEngine;
public class playerController : MonoBehaviour
{
    public float speed = 7.0f;
    public float mouseSensitivity = 100.0f;
    public float rotationSpeed = 4.0f; //마우스 회전 속도

    private CharacterController controller;

    private float xRotation = 0.0f; //카메라 상하 회전 각도

    public Transform playerCamera; //메인 카메라

    void Start()
    {
        controller = GetComponent<CharacterController>();

        //Cursor.lockState = CursorLockMode.Locked; //마우스 커서 표시 X
    }

    void Update()
    {
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        //플레이어 이동
        MovePlayer();

        //카메라 회전 처리
        MouseLook();
    }

    void MovePlayer()
    {
        //이동 입력 받기
        float moveX = Input.GetAxis("Horizontal"); // A, D 또는 좌우 방향키
        float moveZ = Input.GetAxis("Vertical");   // W, S 또는 상하 방향키

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
    }

    void MouseLook()
    {
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  //상하 90도 각도로 제한

        // 카메라 상하 회전
        playerCamera.localRotation = Quaternion.Euler(xRotation * 2.0f, 0f, 0f);

        // 플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
    }
}