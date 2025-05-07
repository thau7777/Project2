using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UI_InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Sprite selectedColor, notSelectedColor;

    public int slotIndex;

    private void Awake()
    {
        Deselect();
    }

    // Select slot
    public void Select()
    {
        image.sprite = selectedColor;
    }

    public void Deselect()
    {
        image.sprite = notSelectedColor;
    }

    // Drop item into slot
    public void OnDrop(PointerEventData eventData)
    {
        UI_InventoryItem draggedItem = eventData.pointerDrag.GetComponent<UI_InventoryItem>();

        if (draggedItem == null) return;

        UI_InventoryItem existingItem = transform.GetComponentInChildren<UI_InventoryItem>();

        if (existingItem != null)
        {
            if (existingItem.InventoryItem.Item == draggedItem.InventoryItem.Item && 
                existingItem.InventoryItem.Item.stackable &&
                existingItem.InventoryItem.Quantity < existingItem.InventoryItem.MaxStack)  
            {
                if(existingItem.InventoryItem.Quantity + draggedItem.InventoryItem.Quantity <= existingItem.InventoryItem.MaxStack)
                {
                    existingItem.InventoryItem.IncreaseQuantity(draggedItem.InventoryItem.Quantity);
                    existingItem.RefreshCount();
                    InventoryManager.Instance.RemoveItemById(draggedItem.InventoryItem);
                    Destroy(draggedItem.gameObject);
                }
                else
                {
                    int quantityToAdd = existingItem.InventoryItem.MaxStack - existingItem.InventoryItem.Quantity;
                    existingItem.InventoryItem.IncreaseQuantity(quantityToAdd);
                    existingItem.RefreshCount();
                    draggedItem.InventoryItem.DecreaseQuantity(quantityToAdd);
                    draggedItem.RefreshCount();
                }

                
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
            //InventoryItem inventoryItem = InventoryManager.Instance.GetItemInSlot(slotIndex);
            //if (inventoryItem != null)
            //{
            //    inventoryItem.UpdateSlotIndex(draggedItem.InventoryItem.SlotIndex);
            //    draggedItem.parentAfterDrag = transform;
            //}
            //else
            draggedItem.parentAfterDrag = transform;
        }
    }
}