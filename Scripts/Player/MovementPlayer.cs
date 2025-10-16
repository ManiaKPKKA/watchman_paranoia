using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour

{
    // Переменные для движения
    public float walkSpeed = 5.0f;
    public float runSpeed = 8.0f;
    private float currentSpeed;

    // Переменные для камеры
    public Transform playerCamera;
    public float mouseSensitivity = 2.0f;
    private float cameraVerticalRotation = 0f;

    // Компонент персонажа
    private CharacterController characterController;

    void Start()
    {
        // Получаем компонент "Контроллер Персонажа"
        characterController = GetComponent<CharacterController>();

        // Заблокировать и скрыть курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // === ВРАЩЕНИЕ КАМЕРЫ (Мышь) ===
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вращение тела игрока влево-вправо (по оси Y)
        transform.Rotate(0, mouseX, 0);

        // Вращение камеры вверх-вниз (по оси X)
        cameraVerticalRotation -= mouseY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f); // Ограничиваем угол
        playerCamera.localRotation = Quaternion.Euler(cameraVerticalRotation, 0, 0);

        // === ДВИЖЕНИЕ (WASD) ===
        float moveForward = Input.GetAxis("Vertical"); // W/S или Стрелка Вверх/Вниз
        float moveRight = Input.GetAxis("Horizontal"); // A/D или Стрелка Влево/Вправо

        // Бег (зажимаем Shift)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Создаём вектор движения ОТНОСИТЕЛЬНО поворота камеры
        Vector3 movement = (transform.forward * moveForward) + (transform.right * moveRight);

        // Применяем скорость и двигаем персонажа через CharacterController
        characterController.SimpleMove(movement * currentSpeed);
    }
}
