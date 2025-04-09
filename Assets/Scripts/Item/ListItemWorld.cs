using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListItemWorld
{
    [SerializeField] List<ItemWorld> _items;
    
    public List<ItemWorld> Items
    { get { return _items; } }

    public ListItemWorld() 
    {
        _items = new List<ItemWorld>();
    }

    public void AddItemWorld(ItemWorld item)
    {
        _items.Add(item);
    }

    public void RemoveItemWorld(ItemWorld item)
    {
        _items.Remove(item);
    }
}
