using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryManagerSO", menuName = "ScriptableObject/SOManager/InventoryManagerSO")]
public class InventoryManagerSO : ScriptableObject
{
    public Inventory inventory;
    public InventoryItem middleInventoryItem;

    public event Func<Transform> onFindEmptySlot;
    public event Action onChangedSelectedSlot;
    public event Action<InventoryItem, int> onAddItemByMouseInteract;
    private int _selectedSlot = 0;
    public int selectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value;
            RefreshCurrentHoldingItem();
        }
    }

    public bool IsMouseOverFadeZone = false;
    public UI_InventoryItem currentDraggingItem = null;
    public Item GetCurrentItem()
    {
        InventoryItem item = inventory.GetInventoryItemOfIndex(_selectedSlot);
        if (item != null)
        {
            return item.Item;
        }
        return null;
    }

    public InventoryItem GetItemInSlot(int index)
    {
        return inventory.FindItemInInventory(index);
    }

    public void RefreshCurrentHoldingItem()
    {
        onChangedSelectedSlot?.Invoke();
    }

    public void RemoveItemById(InventoryItem item)
    {
        inventory.RemoveItemById(item);
    }

    public Transform FindEmptySlot()
    {
        Transform emptySlot = onFindEmptySlot.Invoke();

        return emptySlot;
    }

    public void AddItemToSpecificSlot(InventoryItem item, int slotIndex) // for mouse interact on item
    {
        onAddItemByMouseInteract?.Invoke(item, slotIndex);
        inventory.AddItemToInventory(item, slotIndex);
    }
}
