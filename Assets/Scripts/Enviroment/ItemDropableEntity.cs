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

    protected void Start()
    {
        
    }
    

    public virtual void OnHit(int damage, Vector2 knockback) { }


    

    public void DropItem(bool makeLessDrop)
    {
        RequestToDropItemServerRpc(makeLessDrop);
    }

    [ServerRpc]
    private void RequestToDropItemServerRpc(bool makeLessDrop)
    {
        itemDropPrefab.GetComponent<ItemWorldControl>().item = entityInfo.ItemToDrop;
        int numItem = 0;
        numItem = UtilsClass.PickOneByRatio(entityInfo.numOfItemCouldDrop, entityInfo.ratioForEachNum);
        if (makeLessDrop) numItem /= 2;
        if (numItem > 0)
        {
            for (int i = 0; i < numItem; i++)
            {
                Vector3 randomDir = UtilsClass.GetRandomDir();
                Vector3 position = this.transform.position + randomDir * 0.2f;

                GameObject obj = Instantiate(itemDropPrefab, position, Quaternion.identity);
                obj.GetComponent<NetworkObject>().Spawn();
                SetItemDropStatusClientRpc(obj.GetComponent<NetworkObject>(), randomDir);


                ItemWorld itemWorld = obj.GetComponent<ItemWorldControl>().GetItemWorld();
                itemWorld.SetId();
                ItemWorldManager.Instance.AddItemWorld(itemWorld);

            }
        }
    }

    [ClientRpc]
    private void SetItemDropStatusClientRpc(NetworkObjectReference itemNetworkObject, Vector3 randomDir)
    {
        if(itemNetworkObject.TryGet(out NetworkObject obj))
        {
            var itemWorldControl = obj.GetComponent<ItemWorldControl>();
            itemWorldControl.StartWaitForPickup();
            itemWorldControl.GetComponent<Rigidbody2D>().AddForce(randomDir * 5f, ForceMode2D.Impulse);


        }
    }

}
