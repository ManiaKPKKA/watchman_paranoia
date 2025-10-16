using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    // ���������� ��� ��������
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    private float currentSpeed;

    // ���������� ��� ������
    public Transform playerCamera;
    public float mouseSensitivity = 2.0f;
    private float cameraVerticalRotation = 0f;

    // ��������� ���������
    private CharacterController characterController;

    void Start()
    {
        // �������� ��������� "���������� ���������"
        characterController = GetComponent<CharacterController>();

        // ������������� � ������ ������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // === �������� ������ (����) ===
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // �������� ���� ������ �����-������ (�� ��� Y)
        transform.Rotate(0, mouseX, 0);

        // �������� ������ �����-���� (�� ��� X)
        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f); // ������������ ����
        playerCamera.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);

        // === �������� (WASD) ===
        float moveForward = Input.GetAxis("Vertical"); // W/S ��� ������� �����/����
        float moveRight = Input.GetAxis("Horizontal"); // A/D ��� ������� �����/������

        // ��� (�������� Shift)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // ������ ������ �������� ������������ �������� ������
        Vector3 movement = (transform.forward * moveForward) + (transform.right * moveRight);

        // ��������� �������� � ������� ��������� ����� CharacterController
        characterController.SimpleMove(movement * currentSpeed);
    }
}
