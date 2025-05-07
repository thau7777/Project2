using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorldManager : Singleton<ItemWorldManager>, IDataPersistence
{
    private ListItemWorld _listItemWorld;
    public GameObject itemPrefab;
    public ItemWorldControl[] itemsOnMap;

    public void SpawnItem()
    {
        foreach (var item in _listItemWorld.Items)
        {
            if (!item.IsColected)
            {
                GameObject itemGO = Instantiate(itemPrefab, item.Position, Quaternion.identity);
                ItemWorldControl itemWorldControl = itemGO.GetComponent<ItemWorldControl>();
                itemWorldControl.SetItemWorld(item);
            }
        }
    }

    public void AddItemWorld(ItemWorld item)
    {
        _listItemWorld.AddItemWorld(item);
    }

    public void DropItemFromInventory(InventoryItem item)
    {
        //Vector3 randomDir = UtilsClass.GetRandomDir();
        //Vector3 playerPos = 
        //Vector3 position = playerPos + randomDir * 0.5f;
        //ItemWorld newItemWorld = new ItemWorld(item.Id, item.Item, item.Quantity, position);

        //GameObject transform = Instantiate(itemPrefab, position, Quaternion.identity);

        //transform.gameObject.GetComponent<Rigidbody2D>().AddForce(randomDir * 5f, ForceMode2D.Impulse);
        //ItemWorldControl itemWorldControl = transform.GetComponent<ItemWorldControl>();
        //itemWorldControl.SetItemWorld(newItemWorld);
        //itemWorldControl.StartWaitForPickedup();

        //AddItemWorld(newItemWorld);
    }
    
    public void LoadData(GameData gameData)
    {
        _listItemWorld = gameData.ListItemWold;

        itemsOnMap = FindObjectsOfType<ItemWorldControl>();

        if (_listItemWorld.Items == null || _listItemWorld.Items.Count == 0)
        {
            _listItemWorld = new ListItemWorld();

            foreach (var item in itemsOnMap)
            {
                ItemWorld itemWorld = item.GetItemWorld();
                _listItemWorld.AddItemWorld(itemWorld);
            }
        }
        else
        {
            ItemDatabase.Instance.SetItem(_listItemWorld.Items);

            foreach (var item in itemsOnMap)
            {
                bool existItem = _listItemWorld.Items.Find(x => x.Id == item.id) != null ? true : false;
                if (!existItem)
                {
                    ItemWorld itemWorld = item.GetItemWorld();
                    _listItemWorld.AddItemWorld(itemWorld);
                }
                Destroy(item.gameObject);
            }

            SpawnItem();
        }    
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.SetListItemWorld(_listItemWorld);
    }
}
