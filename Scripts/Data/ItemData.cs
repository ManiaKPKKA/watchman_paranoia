using UnityEngine;

// Эта строка создает пункт в меню создания ассетов
[CreateAssetMenu(fileName = "New Item", menuName = "Bell Tower/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemId;
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public bool isConsumable = false;

    [Header("3D Model")]
    public GameObject worldPrefab; // ← ДОБАВЬ ЭТО ПОЛЕ!

    [Header("Description")]
    [TextArea]
    public string description;
}
public enum ItemType
{
    Key,        // Ключи
    Consumable, // Расходники (свечи, факелы)
    Utility,    // Утилиты (кресты, святая вода)
    Note        // Записки
}