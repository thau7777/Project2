using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemWorld : IItemHolder
{
    [NonSerialized] private Item _item;
    [SerializeField] private string _id;
    [SerializeField] private string _itemName;
    [SerializeField] private int _quantity;
    [SerializeField] private Vector3 _position;
    [SerializeField] private bool _isColected;
   
    public Item Item
    { get { return _item; } }

    public string Id
    { get { return _id; } }

    public string ItemName
    { get { return _itemName; } }

    public int Quantity
    { get { return _quantity; } }

    public Vector3 Position
    { get { return _position; } }

    public bool IsColected
    { get { return _isColected; } }

    public ItemWorld(string id, Item item, int quantity, Vector3 position)
    {
        this._id = id;
        this._item = item;
        this._itemName = item.itemName;
        this._quantity = quantity;
        this._position = position;
        this._isColected = false;
    }

    public void SetItem(Item item)
    {
        this._item = item;
    }

    public void SetId()
    {
        this._id = System.Guid.NewGuid().ToString();
    }

    public void SetId(string id)
    {
        this._id = id;
    }

    public void SetColected(bool colected)
    {
        this._isColected = colected;
    }

    public void SetQuantity(int amount)
    {
        this._quantity = amount;
    }
}
