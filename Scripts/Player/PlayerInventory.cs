using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public System.Action OnInventoryChanged;
    public System.Action<ItemData> OnItemSelected;

    private ItemData[] slots = new ItemData[4];
    private int selectedSlot = -1;

    public bool AddItem(ItemData itemData)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = itemData;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public ItemData GetItemInSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
            return slots[slotIndex];
        return null;
    }

    public void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            selectedSlot = slotIndex;
            ItemData selectedItem = slots[selectedSlot];

            Debug.Log(selectedItem != null ?
                $"✅ Выбран предмет: {selectedItem.itemName}" :
                "✅ Слот очищен");

            OnItemSelected?.Invoke(selectedItem);
        }
    }

    public void UseItem(int slotIndex)
    {
        SelectSlot(slotIndex);
    }

    public ItemData GetSelectedItem()
    {
        return (selectedSlot >= 0) ? slots[selectedSlot] : null;
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    public bool HasItem(string itemId)
    {
        foreach (ItemData item in slots)
        {
            if (item != null && item.itemId == itemId)
                return true;
        }
        return false;
    }

    public void RemoveItemById(string itemId)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].itemId == itemId)
            {
                Debug.Log($"🗑️ Удалён предмет: {slots[i].itemName}");
                slots[i] = null;
                OnInventoryChanged?.Invoke();
                return;
            }
        }
        Debug.Log($"❌ Предмет {itemId} не найден для удаления");
    }

    // ВЫБРОС предмета из выбранного слота
    public void DropSelectedItem()
    {
        if (selectedSlot >= 0 && slots[selectedSlot] != null)
        {
            ItemData itemToDrop = slots[selectedSlot];
            Debug.Log($"🗑️ Выброшен: {itemToDrop.itemName}");

            // СОЗДАЁМ предмет в мире
            CreateDroppedItemInWorld(itemToDrop);

            // Очищаем слот
            slots[selectedSlot] = null;
            selectedSlot = -1;

            OnInventoryChanged?.Invoke();
            OnItemSelected?.Invoke(null);

            NotificationManager.Instance?.ShowNotification($"Выброшен: {itemToDrop.itemName}");
        }
        else
        {
            Debug.Log("❌ Нечего выбрасывать");
            NotificationManager.Instance?.ShowNotification("Нечего выбрасывать");
        }
    }

    // ВЫБРОС предмета из конкретного слота
    public void DropItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length && slots[slotIndex] != null)
        {
            ItemData itemToDrop = slots[slotIndex];
            Debug.Log($"🗑️ Выброшен: {itemToDrop.itemName} из слота {slotIndex}");

            CreateDroppedItemInWorld(itemToDrop);

            slots[slotIndex] = null;

            if (selectedSlot == slotIndex)
            {
                selectedSlot = -1;
                OnItemSelected?.Invoke(null);
            }

            OnInventoryChanged?.Invoke();
            NotificationManager.Instance?.ShowNotification($"Выброшен: {itemToDrop.itemName}");
        }
    }

    // СОЗДАНИЕ предмета в мире с оригинальной моделью
    private void CreateDroppedItemInWorld(ItemData itemData)
    {
        if (itemData.worldPrefab != null)
        {
            Vector3 safeDropPosition = CalculateSafeDropPosition();
            GameObject droppedItem = Instantiate(itemData.worldPrefab, safeDropPosition, Quaternion.identity);
            droppedItem.name = $"Dropped_{itemData.itemName}";

            SetupDroppedItemPhysics(droppedItem, itemData);
            return;
        }

        // Если префаба нет - создаём простой объект с коллайдером
        CreateSimpleDroppedItem(itemData);
    }

    private void SetupDroppedItemPhysics(GameObject droppedItem, ItemData itemData)
    {
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        if (rb == null) rb = droppedItem.AddComponent<Rigidbody>();

        rb.mass = 1.0f;
        rb.linearDamping = 1.0f;
        rb.angularDamping = 0.5f;

        Vector3 throwDirection = transform.forward + Vector3.up * 0.3f;
        rb.AddForce(throwDirection * 0.5f, ForceMode.Impulse);

        InteractableItem interactable = droppedItem.GetComponent<InteractableItem>();
        if (interactable != null)
        {
            interactable.itemData = itemData;
            interactable.interactText = $"Подобрать {itemData.itemName}";
        }
    }
    // РАСЧЁТ БЕЗОПАСНОЙ ПОЗИЦИИ (чтобы не застрять в стене)
    private Vector3 CalculateSafeDropPosition()
    {
        Vector3 idealPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;

        // Проверяем нет ли стены перед игроком
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, 1.5f))
        {
            // Если есть препятствие - бросаем предмет под ноги
            Debug.Log("⚠️ Обнаружена стена, бросаем предмет под ноги");
            return transform.position + transform.forward * 0.5f + Vector3.up * 0.1f;
        }

        return idealPosition;
    }

    // ДОБАВЬ ЭТОТ МЕТОД - он был пропущен!
    private void CreateSimpleDroppedItem(ItemData itemData)
    {
        Vector3 safeDropPosition = CalculateSafeDropPosition();

        GameObject droppedItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
        droppedItem.name = $"Dropped_{itemData.itemName}";
        droppedItem.transform.position = safeDropPosition;
        droppedItem.transform.localScale = Vector3.one * 0.3f;

        // Настраиваем материал
        Renderer renderer = droppedItem.GetComponent<Renderer>();
        renderer.material.color = Color.red;

        // Коллайдер
        Collider collider = droppedItem.GetComponent<Collider>();
        collider.isTrigger = true;

        // ДОБАВЛЯЕМ ФИЗИКУ ДЛЯ ПРОСТОГО ПРЕДМЕТА
        Rigidbody rb = droppedItem.AddComponent<Rigidbody>();
        rb.mass = 1.0f;
        rb.linearDamping = 1.0f;    // Было: rb.drag

        // Бросаем вперёд
        Vector3 throwDirection = transform.forward + Vector3.up * 0.3f;
        rb.AddForce(throwDirection * 1f, ForceMode.Impulse);

        // Логика подбора
        InteractableItem interactable = droppedItem.AddComponent<InteractableItem>();
        interactable.itemData = itemData;
        interactable.interactText = $"Подобрать {itemData.itemName}";

        Debug.Log($"🔄 Создан простой выброшенный предмет с физикой: {itemData.itemName}");
    }

}