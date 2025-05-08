using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class ItemWorldManager : Singleton<ItemWorldManager>, IDataPersistence
{
    private ListItemWorld _listItemWorld;
    public GameObject itemDropPrefab;
    public ItemWorldControl[] itemsOnMap;
    public ItemWorld itemWorldInfoKeeper;
    public void SpawnItem()
    {
        foreach (var item in _listItemWorld.Items)
        {
            if (!item.IsColected)
            {
                GameObject itemGO = Instantiate(itemDropPrefab, item.Position, Quaternion.identity);
                ItemWorldControl itemWorldControl = itemGO.GetComponent<ItemWorldControl>();
                itemWorldControl.InitialItemWorld(item);
            } 
        }
    }

    public void AddItemWorld(ItemWorld item)
    {
        _listItemWorld.AddItemWorld(item);
    }

    public void DropItemFromInventory(InventoryItem item)
    {
        itemWorldInfoKeeper = new ItemWorld(item.Id, item.Item, item.Quantity, Vector3.zero);
        RequestToDropItemServerRpc();

    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestToDropItemServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        NetworkObject senderNetObj = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;

        Vector3 playerPos = senderNetObj.transform.position;
        Vector3 randomDir = UtilsClass.GetRandomDir();
        Vector3 position = playerPos + randomDir * 0.5f;

        itemWorldInfoKeeper.SetPositon(position);

        GameObject newItemDropObj = Instantiate(itemDropPrefab, position, Quaternion.identity);
        var newItemNetworkObject = newItemDropObj.GetComponent<NetworkObject>();
        newItemNetworkObject.Spawn();

        ItemWorldControl itemWorldControl = newItemNetworkObject.GetComponent<ItemWorldControl>();

        itemWorldControl.StartWaitForPickup(5f);
        itemWorldControl.InitialItemWorld(itemWorldInfoKeeper);

        SetItemDropStatusClientRpc(newItemNetworkObject, randomDir);



    }

    [ClientRpc]
    private void SetItemDropStatusClientRpc(NetworkObjectReference newItem, Vector3 randomDir)
    {
        if (newItem.TryGet(out NetworkObject obj))
        {
            obj.GetComponent<Rigidbody2D>().AddForce(randomDir * 5f, ForceMode2D.Impulse);
            AddItemWorld(obj.GetComponent<ItemWorldControl>().GetItemWorld());
        }


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
