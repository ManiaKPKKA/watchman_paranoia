using UnityEngine;

public class InteractableDoorAdvanced : Interactable
{
    [Header("Настройки двери")]
    public bool isLocked = false;
    public string requiredKeyId = "";
    public float maxOpenAngle = 90f;

    [Header("Физические настройки")]
    public float dragForce = 5f;

    private Rigidbody doorRigidbody;
    private HingeJoint doorHinge;
    private bool isBeingDragged = false;

    void Start()
    {
        // Добавляем физику двери
        doorRigidbody = GetComponent<Rigidbody>();
        if (doorRigidbody == null)
        {
            doorRigidbody = gameObject.AddComponent<Rigidbody>();
            doorRigidbody.mass = 10f;
            doorRigidbody.linearDamping = 0.5f;
            doorRigidbody.angularDamping = 0.5f;
        }

        // Настраиваем hinge joint
        doorHinge = GetComponent<HingeJoint>();
        if (doorHinge == null)
        {
            doorHinge = gameObject.AddComponent<HingeJoint>();
            doorHinge.axis = Vector3.up;

            JointLimits limits = new JointLimits();
            limits.min = -maxOpenAngle;
            limits.max = maxOpenAngle;
            doorHinge.limits = limits;
            doorHinge.useLimits = true;
        }
    }

    public override void Interact(GameObject player)
    {
        // Убираем взаимодействие по E - теперь всё через ЛКМ
        // Оставляем метод пустым или можно удалить override
    }

    // Новое: перетаскивание двери мышью
    public void StartDoorDrag(Vector3 dragPosition, GameObject player)
    {
        if (isLocked)
        {
            TryUnlockDoor(player);
            return;
        }

        isBeingDragged = true;
    }

    public void UpdateDoorDrag(Vector3 currentPosition)
    {
        if (!isBeingDragged || isLocked) return;

        // Вычисляем направление для вращения
        Vector3 localOffset = transform.InverseTransformPoint(currentPosition);
        float dragDirection = localOffset.x > 0 ? 1f : -1f;

        // Применяем силу для вращения
        doorRigidbody.AddTorque(transform.up * dragDirection * dragForce);
    }

    public void EndDoorDrag()
    {
        isBeingDragged = false;
    }

    void TryUnlockDoor(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.HasItem(requiredKeyId))
        {
            isLocked = false;
            NotificationManager.Instance?.ShowNotification("Дверь открыта ключом!");
            inventory.RemoveItemById(requiredKeyId);
        }
        else
        {
            NotificationManager.Instance?.ShowNotification("Дверь заперта. Нужен ключ.");
        }
    }

    // Столкновение с игроком
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Игрок может толкать дверь телом
            Vector3 pushForce = collision.relativeVelocity * 2f;
            doorRigidbody.AddForce(pushForce, ForceMode.Impulse);
        }
    }

    public bool CanMonsterPass()
    {
        return !isLocked && Mathf.Abs(transform.localEulerAngles.y) > 10f;
    }
}