using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftingSlot : MonoBehaviour, IDropHandler,  IPointerDownHandler
{
    public int i, j;

    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _inventoryManagerSO.currentDraggingItem != null)
            OnItemDropOnInventorySlot(_inventoryManagerSO.currentDraggingItem);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_inventoryManagerSO.currentDraggingItem != null)
            OnItemDropOnInventorySlot(_inventoryManagerSO.currentDraggingItem);
    }

    public void OnItemDropOnInventorySlot(UI_InventoryItem draggedItem)
    {
        draggedItem.parentAfterDrag = transform;
        draggedItem.OnItemFinishDrag();
    }
    public Item GetItem()
    {
        UI_InventoryItem existingItem = transform.GetComponentInChildren<UI_InventoryItem>();
        return existingItem != null ? existingItem.InventoryItem.Item : null;
    }


}
