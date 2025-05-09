using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DropZone : MonoBehaviour
{
    [SerializeField] private GameEvent onCloseInventory;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;

    //public void OnDrop(PointerEventData eventData)
    //{
    //    UI_InventoryItem draggedItem = eventData.pointerDrag.GetComponent<UI_InventoryItem>();

    //    if (draggedItem == null) return;

    //    draggedItem.parentAfterDrag = transform;
    //}

    public void CloseInventory()
    {
        onCloseInventory.Raise(this, ActionMap.Player);
    }

}
