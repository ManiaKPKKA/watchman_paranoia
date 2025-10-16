using UnityEngine;

public class ItemDragger : MonoBehaviour
{
    [Header("��������� ��������������")]
    public float dragDistance = 3f;
    public float holdDistance = 2f;
    public float throwForce = 5f;
    public LayerMask dragLayers = 1;

    private Camera playerCamera;
    private Rigidbody draggedObject;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private float currentDragDistance;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }

void Update()
{
    HandleDragInput();
    
    if (isDragging)
    {
        // Проверяем расстояние до предмета
        if (IsObjectTooFar())
        {
            EndDrag(false); // Отпускаем без броска
            NotificationManager.Instance?.ShowNotification("Слишком далеко!");
            return;
        }
        
        UpdateDraggedObject();
    }
}

    // Проверка расстояния
    private bool IsObjectTooFar()
    {
        if (!isDragging) return true;

        // Для дверей не проверяем расстояние (они стационарные)
        InteractableDoorAdvanced door = draggedObject?.GetComponent<InteractableDoorAdvanced>();
        if (door != null) return false;

        // Для обычных предметов проверяем расстояние
        if (draggedObject == null) return true;

        float distanceToObject = Vector3.Distance(
            playerCamera.transform.position,
            draggedObject.position
        );

        return distanceToObject > dragDistance * 1.5f;
    }

    void HandleDragInput()
    {
        // Начало перетаскивания
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            TryStartDrag();
        }

        // Бросок ПКМ
        if (Input.GetMouseButtonDown(1) && isDragging)
        {
            ThrowItem();
        }

        // Просто отпустить ЛКМ
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag(false); // false = без броска
        }

        // Регулировка расстояния
        if (isDragging && Input.mouseScrollDelta.y != 0)
        {
            currentDragDistance = Mathf.Clamp(
                currentDragDistance + Input.mouseScrollDelta.y * 0.5f,
                1f,
                dragDistance
            );
        }
    }

    void TryStartDrag()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dragDistance, dragLayers))
        {
            // Пробуем взять дверь
            InteractableDoorAdvanced door = hit.collider.GetComponent<InteractableDoorAdvanced>();
            if (door != null)
            {
                draggedObject = door.GetComponent<Rigidbody>();
                door.StartDoorDrag(hit.point, gameObject); // Передаём игрока
                isDragging = true;
                currentDragDistance = Vector3.Distance(playerCamera.transform.position, hit.point);
                return;
            }

            // Обычные предметы...
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                StartDrag(rb, hit.point);
            }
        }
    }

    void StartDrag(Rigidbody rb, Vector3 hitPoint)
    {
        draggedObject = rb;
        originalPosition = rb.position;
        isDragging = true;

        // ��������� ���������� � ��������� ������������ ���������
        rb.useGravity = false;
        rb.angularDamping = 5f; // ����������� ������� ������������� ��� ������������

        // ��������� ��������� ����������
        currentDragDistance = Vector3.Distance(playerCamera.transform.position, hitPoint);
        currentDragDistance = Mathf.Clamp(currentDragDistance, 1f, dragDistance);

        NotificationManager.Instance?.ShowNotification("Держу");
    }


    void UpdateDraggedObject()
    {
        if (!isDragging) return;

        Vector3 targetPosition = playerCamera.transform.position +
                               playerCamera.transform.forward * currentDragDistance;

        // Для дверей
        InteractableDoorAdvanced door = draggedObject?.GetComponent<InteractableDoorAdvanced>();
        if (door != null)
        {
            door.UpdateDoorDrag(targetPosition);
            return;
        }

        // Для обычных предметов
        if (draggedObject != null)
        {
            Vector3 direction = targetPosition - draggedObject.position;
            float distance = direction.magnitude;

            float maxSpeed = 10f;
            Vector3 targetVelocity = direction * 5f;

            if (targetVelocity.magnitude > maxSpeed)
                targetVelocity = targetVelocity.normalized * maxSpeed;

            draggedObject.linearVelocity = targetVelocity;
            draggedObject.angularVelocity = Vector3.zero;
        }
    }



    void EndDrag(bool throwItem = false)
    {
        if (!isDragging) return;

        // Отпускаем дверь
        InteractableDoorAdvanced door = draggedObject?.GetComponent<InteractableDoorAdvanced>();
        if (door != null)
        {
            door.EndDoorDrag();
        }
        // Обычные предметы...
        else if (draggedObject != null)
        {
            draggedObject.linearVelocity = Vector3.zero;
            draggedObject.angularVelocity = Vector3.zero;
            draggedObject.useGravity = true;
            draggedObject.angularDamping = 0.05f;

            if (throwItem)
            {
                Vector3 throwDirection = playerCamera.transform.forward;
                draggedObject.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
                NotificationManager.Instance?.ShowNotification("Предмет брошен");
            }
            else
            {
                NotificationManager.Instance?.ShowNotification("Предмет отпущен");
            }
        }

        draggedObject = null;
        isDragging = false;
    }

    void ThrowItem()
    {
        EndDrag(true);
    }
    public bool IsDragging()
    {
        return isDragging;
    }

    // ������������ � ���������
    void OnDrawGizmos()
    {
        if (playerCamera != null && isDragging)
        {
            Gizmos.color = Color.green;
            Vector3 targetPos = playerCamera.transform.position +
                              playerCamera.transform.forward * currentDragDistance;
            Gizmos.DrawWireSphere(targetPos, 0.1f);
            Gizmos.DrawLine(playerCamera.transform.position, targetPos);
        }
    }
}