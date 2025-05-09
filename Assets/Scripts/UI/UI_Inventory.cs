using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    // GameEvent

    // UpdateUI
    public void RegisterManagerSOEvent()
    {
        _inventoryManagerSO.onFindEmptySlot += FindEmptySlot;
        _inventoryManagerSO.onAddItemByMouseInteract += AddItemToCurrentItemSlot;
    }

    private void OnDisable()
    {
        _inventoryManagerSO.onFindEmptySlot -= FindEmptySlot;
        _inventoryManagerSO.onAddItemByMouseInteract -= AddItemToCurrentItemSlot;
    }
    public void UpdateSlotUI(Component sender, object data)
    {
        var inventory = _inventoryManagerSO.inventory;
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
            if (inventoryItem != null)
            {
                SpawnItem(inventoryItem, slotUIGO);
            }
        }
    }

    public void ClearSlotUI()
    {
        foreach (UI_InventorySlot slotUI in inventorySlotsUI) Destroy(slotUI.gameObject);
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

    // event stuff =======================================================================
    public void ChangeSelectedSlot(Component sender, object data)
    {
        int newValue = (int)data;
        Debug.Log(newValue);
        inventorySlotsUI[_inventoryManagerSO.selectedSlot].Deselect();
        inventorySlotsUI[newValue].Select();
        _inventoryManagerSO.selectedSlot = newValue;
    }


    public void AddItemToInventorySlot(Component sender, object data) // for pick itemworld
    {
        ItemWorldControl itemWorldControl = sender as ItemWorldControl;
        var newItem = _inventoryManagerSO.middleInventoryItem;
        var inventory = _inventoryManagerSO.inventory;
        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            UI_InventoryItem itemUI = slotUI.GetComponentInChildren<UI_InventoryItem>();

            if (itemUI != null &&
                itemUI.InventoryItem.Item == newItem.Item &&
                itemUI.InventoryItem.Item.stackable &&
                itemUI.InventoryItem.Quantity < itemUI.InventoryItem.MaxStack)
            {
                if (itemUI.InventoryItem.Quantity + newItem.Quantity <= itemUI.InventoryItem.MaxStack)
                {
                    itemUI.InventoryItem.IncreaseQuantity(newItem.Quantity);
                    itemUI.RefreshCount();
                    itemWorldControl.DestroyItemWorld();
                    return;
                }
                else
                {
                    int quantityToAdd = itemUI.InventoryItem.MaxStack - itemUI.InventoryItem.Quantity;
                    itemUI.InventoryItem.IncreaseQuantity(quantityToAdd);
                    itemUI.RefreshCount();
                    newItem.DecreaseQuantity(quantityToAdd);
                }


            }
        }

        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            if (slotUI.transform.childCount == 0)
            {
                inventory.AddItemToInventory(newItem, slotUI.slotIndex);
                AddItemToInventoryUI(newItem, slotUI.slotIndex);
                itemWorldControl.DestroyItemWorld();
                if (_inventoryManagerSO.selectedSlot == i) _inventoryManagerSO.RefreshCurrentHoldingItem();

                return;
            }
        }
        
        return;
    }

    public Transform FindEmptySlot()
    {
        for (int i = 0; i < inventorySlotsUI.Count; i++)
        {
            UI_InventorySlot slotUI = inventorySlotsUI[i];
            UI_InventoryItem itemUI = slotUI.GetComponentInChildren<UI_InventoryItem>();
            if (itemUI == null)
            {
                return slotUI.transform;
            }
        }
            return null;
    }

    public void AddItemToSlot(Component sender, object data)
    {
        InventoryItem newItem = _inventoryManagerSO.middleInventoryItem;
        int emptySlotIndex = (int)data;
        _inventoryManagerSO.inventory.AddItemToInventory(newItem, emptySlotIndex);
        AddItemToInventoryUI(newItem, emptySlotIndex);
    }

    public void AddItemToCurrentItemSlot(InventoryItem newItem, int slotIndex) // for split item
    {
        UI_InventorySlot slotUI = inventorySlotsUI[slotIndex];
        AddItemToInventoryUI(newItem, slotUI.slotIndex);
    }
}
