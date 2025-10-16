using UnityEngine;

public class InteractableItem : Interactable
{
    public ItemData itemData;

    [Header("���������� ���������")]
    public float mass = 1.0f;
    public bool enablePhysics = true;

    private Rigidbody rb;

    void Start()
    {
        // ��������� Rigidbody ���� ��� ���
        rb = GetComponent<Rigidbody>();
        if (rb == null && enablePhysics)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.3f;
        }
    }

    public override void Interact(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null && inventory.AddItem(itemData))
        {
            NotificationManager.Instance?.ShowNotification($"��������: {itemData.itemName}");

            // ���� ���� Rigidbody - ������� ������ ����� ������������
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Destroy(gameObject);
        }
        else
        {
            NotificationManager.Instance?.ShowNotification("��������� �����!");
        }
    }

    // ����� ��� ��������������� ���������/���������� ������
    public void SetPhysicsEnabled(bool enabled)
    {
        if (rb != null)
        {
            rb.isKinematic = !enabled;
        }
    }
}