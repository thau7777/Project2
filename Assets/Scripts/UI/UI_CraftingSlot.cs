using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftingSlot : MonoBehaviour, IDropHandler
{
    public int i, j;

    public void OnDrop(PointerEventData eventData)
    {
        UI_InventoryItem draggedItem = eventData.pointerDrag.GetComponent<UI_InventoryItem>();

        if (draggedItem == null) return;

        UI_InventoryItem existingItem = transform.GetComponentInChildren<UI_InventoryItem>();

        if (existingItem != null)
        {
            if (existingItem.InventoryItem.Item.itemName == draggedItem.InventoryItem.Item.itemName && 
                existingItem.InventoryItem.Item.stackable &&
                existingItem.InventoryItem.Quantity < existingItem.InventoryItem.MaxStack)  
            {
                existingItem.InventoryItem.IncreaseQuantity(draggedItem.InventoryItem.Quantity);
                existingItem.RefreshCount();

                InventoryManager.Instance.RemoveItemById(draggedItem.InventoryItem);
                Destroy(draggedItem.gameObject);
                return;
            }
            else if (existingItem.InventoryItem.Item.itemName != draggedItem.InventoryItem.Item.itemName)
            {
                existingItem.ChangeSlot(draggedItem.parentAfterDrag, draggedItem.InventoryItem.SlotIndex);
                draggedItem.parentAfterDrag = transform;
            }
        }
        else
        {
            draggedItem.parentAfterDrag = transform;
        }
    }

    public Item GetItem()
    {
        UI_InventoryItem existingItem = transform.GetComponentInChildren<UI_InventoryItem>();
        return existingItem != null ? existingItem.InventoryItem.Item : null;
    }
}
