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
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private Collider2D _TargetzoneCollider2D;

    public NetworkVariable<bool> CanPickup = new NetworkVariable<bool>(true);

    //private bool _isSpawned = false;
    //public bool IsSpawned
    //{
    //    get { return _isSpawned; }
    //    private set { _isSpawned = value; }
    //}
    public Transform targetTransform;

    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxSpeed;

    private Vector3 _currentVelocity = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        InitialItemWorld();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanPickup.OnValueChanged += UpdateCollider;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        CanPickup.OnValueChanged -= UpdateCollider;
    }
    private void FixedUpdate()
    {
        if (targetTransform == null)
        {
            _currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
        else
        {
            Vector3 currentPos = rb.position;
            Vector3 direction = (targetTransform.position - currentPos).normalized;

            _currentVelocity = direction * _acceleration;
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, _maxSpeed);
            _acceleration += 0.1f;
            rb.velocity = _currentVelocity;
        }
        
    }

    public void SetTargetTransform(Transform playerTransform)
    {
        targetTransform = playerTransform;
    }

    public void SetItemImage(Sprite image)
    {
        transform.GetComponent<SpriteRenderer>().sprite = item.image;
    }

    public ItemWorld GetItemWorld()
    {
        return _itemWorld;
    }

    public void InitialItemWorld(ItemWorld itemWorld = null)
    {
        if(itemWorld == null)
        {
            itemWorld = new ItemWorld(System.Guid.NewGuid().ToString(), item, 1, transform.position);
        }
        _itemWorld = itemWorld;
        id = itemWorld.Id;
        item = itemWorld.Item;
        SetItemImage(item.image);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && CanPickup.Value)
        {
            collision.GetComponent<InventoryController>().AddItemWorldToInventory(this);
        }
    }

    public void DestroyItemWorld()
    {
        if (IsServer)
            RequestToDestroyObjectServerRpc(GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToDestroyObjectServerRpc(NetworkObjectReference objRef)
    {
        SetColectedForClientRpc(objRef);
        if (objRef.TryGet(out NetworkObject obj))
        {
            Destroy(obj.gameObject);
        }
        
    }

    [ClientRpc]
    private void SetColectedForClientRpc(NetworkObjectReference objRef)
    {
        if (objRef.TryGet(out NetworkObject obj))
        {
            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl._itemWorld.SetColected(true);
        }
    }


    public void StartWaitForPickup(float timesTillCanPickup)
    {
        StartCoroutine(WaitForPickup(timesTillCanPickup));
    }

    IEnumerator WaitForPickup(float timesTillCanPickup)
    {
        CanPickup.Value = false;
        yield return new WaitForSeconds(timesTillCanPickup);
        CanPickup.Value = true;
    }

    private void UpdateCollider(bool oldValue, bool newValue)
    {
        if(newValue)
        {
            _collider2D.enabled = true;
            _TargetzoneCollider2D.enabled = true;
        }
        else
        {
            _collider2D.enabled = false;
            _TargetzoneCollider2D.enabled = false;
        }
    }
    

}


