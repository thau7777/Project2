using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UI_Inventory : MonoBehaviour
{
    public Transform toolBarContainer;
    public Transform inventoryContainer;
    public GameObject slotPrefab;
    public GameObject UI_inventoryItemPrefab;
    public List<UI_InventorySlot> inventorySlotsUI;
    private int maxToolBarSlot = 9;

    // UpdateUI
    public void UpdateSlotUI(Inventory inventory)
    {
        ClearSlotUI();
        int totalSlots = inventory.MaxSlotInventory;

        for (int i = 0; i < totalSlots; i++)
        {
            Transform parent = (i < maxToolBarSlot) ? toolBarContainer : inventoryContainer;
            GameObject slotUIGO = Instantiate(slotPrefab, parent);
            UI_InventorySlot inventoryslotUI = slotUIGO.GetComponent<UI_InventorySlot>();
            inventoryslotUI.slotIndex = i;
            inventorySlotsUI.Add(inventoryslotUI);

            InventoryItem inventoryItem = inventory.GetInventoryItemOfIndex(i);
            if (inventoryItem != null )
            {
                SpawnItem(inventoryItem, slotUIGO);
            }
        }
    }

    public void ClearSlotUI()
    {
        foreach(UI_InventorySlot slotUI in inventorySlotsUI) Destroy(slotUI.gameObject);
        inventorySlotsUI.Clear();
    }

    // AddItemUI
    public bool AddItemToInventoryUI(InventoryItem inventoryItem, int index)
    {
        UI_InventorySlot slotUI = inventorySlotsUI[index];
        SpawnItem(inventoryItem, slotUI.gameObject);
        return true;
    }

    public void SpawnItem(InventoryItem item, GameObject slot)
    {
        GameObject newItemGO = Instantiate(UI_inventoryItemPrefab, slot.transform);
        UI_InventoryItem inventoryItem = newItemGO.GetComponent<UI_InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }
}
