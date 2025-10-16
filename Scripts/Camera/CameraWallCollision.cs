using UnityEngine;

public class CameraWallCollision : MonoBehaviour
{
    [Header("Настройки столкновений")]
    public float clipOffset = 0.1f;
    public float minCameraDistance = 0.5f;
    public LayerMask collisionLayer = 1; // По умолчанию все слои

    private Vector3 originalLocalPos;
    private Transform playerTransform;

    void Start()
    {
        // Запоминаем оригинальную позицию камеры относительно игрока
        originalLocalPos = transform.localPosition;
        playerTransform = transform.parent;
    }

    void LateUpdate()
    {
        HandleCameraCollision();
    }

    void HandleCameraCollision()
    {
        // Тarget позиция камеры (где она должна быть без столкновений)
        Vector3 targetPos = playerTransform.TransformPoint(originalLocalPos);
        Vector3 direction = targetPos - playerTransform.position;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, direction, out hit, distance + clipOffset, collisionLayer))
        {
            // Столкновение - отодвигаем камеру ближе к игроку
            float newDistance = Mathf.Max(hit.distance - clipOffset, minCameraDistance);
            transform.position = playerTransform.position + direction.normalized * newDistance;
        }
        else
        {
            // Нет столкновений - возвращаем камеру на нормальную позицию
            transform.localPosition = originalLocalPos;
        }
    }
}