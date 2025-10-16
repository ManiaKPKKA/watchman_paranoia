using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Ссылки на слоты")]
    public Image[] slotIcons = new Image[4];
    public Image[] slotFrames = new Image[4];

    private PlayerInventory inventory;

    void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();

        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateUI;
            inventory.OnItemSelected += OnItemSelectedHandler;
        }
        UpdateUI();
    }

    // ПЕРЕИМЕНОВАЛ - было Update, стало InventoryUpdate
    void InventoryUpdate()
    {
        // ГОРЯЧИЕ КЛАВИШИ 1-4
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectInventorySlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectInventorySlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectInventorySlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectInventorySlot(3);

        // ВЫБРОС ПРЕДМЕТА - КЛАВИША G
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("🎹 Нажата G - выброс предмета");
            DropSelectedItem();
        }

        // ПРОКРУТКА КОЛЁСИКОМ МЫШИ
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int currentSlot = inventory.GetSelectedSlot();
            int newSlot = currentSlot;

            if (scroll > 0) // Вперёд
                newSlot = (currentSlot + 1) % 4;
            else if (scroll < 0) // Назад
                newSlot = (currentSlot - 1 + 4) % 4;

            SelectInventorySlot(newSlot);
        }
    }

    // ПЕРЕИМЕНОВАЛ - было SelectSlot, стало SelectInventorySlot
    void SelectInventorySlot(int slotIndex)
    {
        if (inventory != null)
            inventory.SelectSlot(slotIndex);
    }

    // ПЕРЕИМЕНОВАЛ - было OnItemSelected, стало OnItemSelectedHandler
    void OnItemSelectedHandler(ItemData selectedItem)
    {
        int selectedSlot = inventory.GetSelectedSlot();

        for (int i = 0; i < 4; i++)
        {
            if (slotFrames[i] != null)
            {
                if (i == selectedSlot)
                    slotFrames[i].color = Color.yellow;
                else
                    slotFrames[i].color = Color.white;
            }
        }
    }

    public void UpdateUI()
    {
        if (inventory == null) return;

        for (int i = 0; i < 4; i++)
        {
            ItemData item = inventory.GetItemInSlot(i);

            if (item != null && slotIcons[i] != null)
            {
                slotIcons[i].sprite = item.icon;
                slotIcons[i].color = Color.white;
            }
            else if (slotIcons[i] != null)
            {
                slotIcons[i].sprite = null;
                slotIcons[i].color = Color.clear;
            }
        }
    }

    // Выброс выбранного предмета
    void DropSelectedItem()
    {
        if (inventory != null)
            inventory.DropSelectedItem();
    }

    // Клик по слоту (для UI кнопок) - оставляем старое имя для совместимости
    public void OnSlotClick(int slotIndex)
    {
        SelectInventorySlot(slotIndex);
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateUI;
            inventory.OnItemSelected -= OnItemSelectedHandler;
        }
    }
    void Update()
    {
        InventoryUpdate();
    }
}