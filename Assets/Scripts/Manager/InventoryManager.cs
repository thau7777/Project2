using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : Singleton<InventoryManager>, IDataPersistence
{
    private Inventory inventory;
    
    public UI_Inventory inventoryUI;

    private static int selectedSlot = -1;

    private void Update()
    {
        if (Input.inputString != null)
        {
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if (isNumber && number > 0 && number <= 9)
            {
                ChangeSelectedSlot(number - 1);
            }
        }
    }

    void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventoryUI.inventorySlotsUI[selectedSlot].Deselect();
        }

        inventoryUI.inventorySlotsUI[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItemToInventory(ItemWorld item)
    {
        InventoryItem inventoryItem = new InventoryItem(item.Id, item.Item, 0, item.Quantity);
        return AddItemToInventorySlot(inventoryItem);
    }    

    public bool AddItemToInventory(InventoryItem item, int slotIndex)
    {
        return inventory.AddItemToInventory(item, slotIndex);
    }    

    public bool AddItemToInventorySlot(InventoryItem newItem)
    {
        for (int i = 0; i < inventoryUI.inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventoryUI.inventorySlotsUI[i];
            UI_InventoryItem itemUI = slotUI.GetComponentInChildren<UI_InventoryItem>();

            if (itemUI != null &&
                itemUI.InventoryItem.Item == newItem.Item &&
                itemUI.InventoryItem.Quantity < itemUI.InventoryItem.MaxStack &&
                itemUI.InventoryItem.Item.stackable)
            {
                itemUI.InventoryItem.IncreaseQuantity(newItem.Quantity);
                itemUI.RefreshCount();
                inventoryUI.UpdateSlotUI(inventory);
                return true;
            }
        }

        for (int i = 0; i < inventoryUI.inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventoryUI.inventorySlotsUI[i];
            if (slotUI.transform.childCount == 0)
            {
                inventory.AddItemToInventory(newItem, slotUI.slotIndex);
                inventoryUI.AddItemToInventoryUI(newItem, slotUI.slotIndex);
                return true;
            }
        }

        return false;
    }


    public Item GetSelectedItem(bool use)
    {
        UI_InventorySlot slot = inventoryUI.inventorySlotsUI[selectedSlot];
        UI_InventoryItem itemInSlot = slot.GetComponentInChildren<UI_InventoryItem>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.InventoryItem.Item;
            if (use == true)
            {
                itemInSlot.InventoryItem.DecreaseQuantity(1);
                if (itemInSlot.InventoryItem.Quantity <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
                else
                {
                    itemInSlot.RefreshCount();
                }
            }

            return item;
        }

        return null;
    }

    public UI_InventorySlot GetEmptySlot()
    {
        foreach (var slot in inventoryUI.inventorySlotsUI)
        {
            UI_InventoryItem inventoryItemUI = slot.GetComponentInChildren<UI_InventoryItem>();
            if (inventoryItemUI == null) return slot;
        }
        return null;
    }

    public void AddItemToEmptySlot(InventoryItem newItem, UI_InventorySlot emptySlot)
    {
        inventory.AddItemToInventory(newItem, emptySlot.slotIndex);
        inventoryUI.AddItemToInventoryUI(newItem, emptySlot.slotIndex);
    }

    public void RemoveItemById(InventoryItem item)
    {
        inventory.RemoveItemById(item);
    }

    public InventoryItem GetItemInSlot(int index)
    {
        return inventory.FindItemInInventory(index);
    }

    public void LoadData(GameData gameData)
    {
        inventory = gameData.InventoryData;
        ItemDatabase.Instance.SetItem(inventory.InventoryItemList);
        inventoryUI.UpdateSlotUI(inventory);
        ChangeSelectedSlot(0);
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.SetInventoryData(inventory);
    }
}
