using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class Inventory
{
    [SerializeField] private int _maxSlotInventory;
    [SerializeField] private List<InventoryItem> _ineventoryItemList;

    public int MaxSlotInventory
    {  get { return _maxSlotInventory; } }

    public List<InventoryItem> InventoryItemList
    {  get { return _ineventoryItemList; } }

    public Inventory() 
    {
        this._maxSlotInventory = 44;
        this._ineventoryItemList = new List<InventoryItem>(_maxSlotInventory);
    }

    public InventoryItem GetInventoryItemOfIndex(int index)
    {
        InventoryItem item = _ineventoryItemList.Find(i => i.SlotIndex == index);
        return item;
    }

    public bool AddItemToInventory(InventoryItem item, int slotIndex)
    {
        item.UpdateSlotIndex(slotIndex);
        _ineventoryItemList.Add(item);
        return true;
    }

    public bool AddItemToInventory(string id, Item item, int slotIndex, int amount)
    {
        InventoryItem inventoryItem = new InventoryItem(id, item, slotIndex, amount);
        _ineventoryItemList.Add(inventoryItem);
        return true;
    }

    public bool RemoveItemFromInventory(Item item, int amount = 1)
    {
        InventoryItem existingItem = _ineventoryItemList.Find(i => i.Item == item);
        if (existingItem != null)
        {
            existingItem.DecreaseQuantity(amount);
            if (existingItem.Quantity <= 0) _ineventoryItemList.Remove(existingItem); return true;
        }
        return false;
    }

    public bool RemoveItemById(InventoryItem inventoryItem)
    {
        InventoryItem existingItem = _ineventoryItemList.Find(i => i.Id == inventoryItem.Id);
        if (existingItem != null)
        {
            _ineventoryItemList.Remove(existingItem);
            return true;
        }
        return false;
    }

    public InventoryItem FindItemInInventory(int index)
    {
        InventoryItem existingItem = _ineventoryItemList.Find(i => i.SlotIndex == index);
        if (existingItem != null) return existingItem;
        return null;
    }
}
