using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class InventoryItem : IItemHolder
{
    [NonSerialized] private Item _item;
    [SerializeField] private string _id;
    [SerializeField] private string _itemName;
    [SerializeField] private int _quantity;
    [SerializeField] private int _maxStack;
    [SerializeField] private int _slotIndex;

    public Item Item
    { get { return _item; } }

    public string Id
    { get { return _id; } }

    public string ItemName
    { get { return _itemName; } }

    public int Quantity
    { get { return _quantity; } }

    public int MaxStack
    { get { return _maxStack; } }

    public int SlotIndex
    {  get { return _slotIndex; } }

    public InventoryItem(string id, Item item, int index)
    {
        this._id = id;
        this._item = item;
        this._itemName = item.itemName;
        this._quantity ++;
        this._maxStack = 12;
        this._slotIndex = index;
    }

    public InventoryItem(string id, Item item, int index, int amount)
    {
        this._id = id;
        this._item = item;
        this._itemName = item.itemName;
        this._quantity += amount;
        this._maxStack = 12;
        this._slotIndex = index;
    }

    public void SetItem(Item item)
    {
        this._item = item;
    }

    public void IncreaseQuantity(int amount)
    {
        _quantity += amount;
    }

    public void DecreaseQuantity(int amount) 
    {
        _quantity -= amount;
        if (_quantity < 0) _quantity = 0;
    }

    public void UpdateSlotIndex(int newIndex)
    {
        _slotIndex = newIndex;
    }
}
