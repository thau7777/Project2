using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(NetworkObject))]
public abstract class ItemDropableEntity : NetworkBehaviour
{
    protected Damageable damageable;
    [SerializeField] protected ItemDropableEntitySO entityInfo;
    [SerializeField] protected GameObject itemDropPrefab;

    protected virtual void Awake()
    {
        damageable = GetComponent<Damageable>();
    }

    public virtual void OnHit(int damage, Vector2 knockback) { }


    public void DropItem(bool makeLessDrop)
    {
        if(!IsServer) return;
        RequestToDropItemServerRpc(makeLessDrop);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToDropItemServerRpc(bool makeLessDrop)
    {
        
        int numItem = 0;
        numItem = UtilsClass.PickOneByRatio(entityInfo.numOfItemCouldDrop, entityInfo.ratioForEachNum);
        Debug.Log("Num of item drop: " + numItem);
        if (makeLessDrop) numItem /= 2;
        if (numItem > 0)
        {
            for (int i = 0; i < numItem; i++)
            {
                Vector3 randomDir = UtilsClass.GetRandomDir();
                Vector3 position = transform.position + randomDir * 0.2f;

                ItemWorld newItemWorldInfo = new(System.Guid.NewGuid().ToString(), entityInfo.ItemToDrop, 1, position);

                GameObject newItemObject = Instantiate(itemDropPrefab, position, Quaternion.identity);
                var newItemNetworkObject = newItemObject.GetComponent<NetworkObject>();
                newItemNetworkObject.Spawn();

                var itemWorldControl = newItemNetworkObject.GetComponent<ItemWorldControl>();
                itemWorldControl.StartWaitForPickup(2f);
                itemWorldControl.InitialItemWorld(newItemWorldInfo);


                SetItemDropStatusClientRpc(newItemNetworkObject, randomDir);

            }
        }
    }

    [ClientRpc]
    private void SetItemDropStatusClientRpc(NetworkObjectReference itemNetworkObject, Vector3 randomDir)
    {
        if(itemNetworkObject.TryGet(out NetworkObject obj))
        {
            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl.GetComponent<Rigidbody2D>().AddForce(randomDir * 1f, ForceMode2D.Impulse);

            ItemWorld itemWorld = itemWorldControl.GetItemWorld();
            
            ItemWorldManager.Instance.AddItemWorld(itemWorld);

        }
    }

}
