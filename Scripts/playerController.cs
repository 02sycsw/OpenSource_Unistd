using UnityEngine;
public class playerController : MonoBehaviour
{
    public float speed = 7.0f;
    public float mouseSensitivity = 100.0f;
    public float rotationSpeed = 4.0f; //���콺 ȸ�� �ӵ�

    private CharacterController controller;

    private float xRotation = 0.0f; //ī�޶� ���� ȸ�� ����

    public Transform playerCamera; //���� ī�޶�

    void Start()
    {
        controller = GetComponent<CharacterController>();

        //Cursor.lockState = CursorLockMode.Locked; //���콺 Ŀ�� ǥ�� X
    }

    void Update()
    {
        if (GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        //�÷��̾� �̵�
        MovePlayer();

        //ī�޶� ȸ�� ó��
        MouseLook();
    }

    void MovePlayer()
    {
        //�̵� �Է� �ޱ�
        float moveX = Input.GetAxis("Horizontal"); // A, D �Ǵ� �¿� ����Ű
        float moveZ = Input.GetAxis("Vertical");   // W, S �Ǵ� ���� ����Ű

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);
    }

    void MouseLook()
    {
        // ���콺 �Է�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  //���� 90�� ������ ����

        // ī�޶� ���� ȸ��
        playerCamera.localRotation = Quaternion.Euler(xRotation * 2.0f, 0f, 0f);

        // �÷��̾� �¿� ȸ��
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
    }
}