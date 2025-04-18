using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemWorldControl : NetworkBehaviour
{
    public string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public Item item;
    private ItemWorld _itemWorld;
    private Rigidbody2D rb;

    public NetworkVariable<bool> CanPickup = new NetworkVariable<bool>(true);

    private bool _isSpawned = false;
    public bool IsSpawned
    {
        get { return _isSpawned; }
        private set { _isSpawned = value; }
    }


    private void Awake()
    {
        InitialItem(item);
        _itemWorld = new ItemWorld(id, item, 1, transform.position);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity *= 0.8f;
        
    }
    public void InitialItem(Item item)
    {
        transform.GetComponent<SpriteRenderer>().sprite = item.image;
    }

    public ItemWorld GetItemWorld()
    {
        return _itemWorld;
    }

    public void SetItemWorld(ItemWorld itemWorld)
    {
        _itemWorld = itemWorld;
        id = itemWorld.Id;
        item = itemWorld.Item;
        InitialItem(item);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CanPickup.Value)
        {
            if (InventoryManager.Instance.AddItemToInventory(_itemWorld))
            {
                _itemWorld.SetColected(true);
                GetComponent<NetworkObject>().Despawn();
                Destroy(gameObject);
            }
        }
    }

    public void StartWaitForPickup()
    {
        RequestStartWaitForPickupServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestStartWaitForPickupServerRpc()
    {
        StartCoroutine(WaitForPickup());
    }
    IEnumerator WaitForPickup()
    {
        CanPickup.Value = false;
        yield return new WaitForSeconds(1.5f);

        CanPickup.Value = true;
    }



}


