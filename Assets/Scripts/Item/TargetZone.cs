using Unity.Netcode;
using UnityEngine;

public class TargetZone : NetworkBehaviour
{
    
    private ItemWorldControl _itemWorldControl;
    public override void OnNetworkSpawn()
    {
        _itemWorldControl = GetComponentInParent<ItemWorldControl>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _itemWorldControl.targetTransform == null && _itemWorldControl.CanPickup.Value)
        {
            RequestSetChasingTransformServerRpc(_itemWorldControl.GetComponent<NetworkObject>(), 
                collision.GetComponent<NetworkObject>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSetChasingTransformServerRpc(NetworkObjectReference itemWorldRef, NetworkObjectReference playerRef)
    {
        SetChasingTransformClientRpc(itemWorldRef, playerRef);
    }

    [ClientRpc]
    private void SetChasingTransformClientRpc(NetworkObjectReference itemWorldRef, NetworkObjectReference playerRef)
    {
        if(itemWorldRef.TryGet(out NetworkObject itemWorldObj) && playerRef.TryGet(out NetworkObject playerObj))
        {
            var itemWorldControl = itemWorldObj.GetComponent<ItemWorldControl>();
            var playerTransform = playerObj.GetComponent<Transform>();
            itemWorldControl.SetTargetTransform(playerTransform);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.transform == _itemWorldControl.targetTransform)
        {
            RequestToRemoveTargetTransformServerRpc(_itemWorldControl.GetComponent<NetworkObject>());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToRemoveTargetTransformServerRpc(NetworkObjectReference itemWorldRef)
    {

        RemoveTargetTransformClientRpc(itemWorldRef);
    }

    [ClientRpc]
    private void RemoveTargetTransformClientRpc(NetworkObjectReference itemWorldRef)
    {
        if (itemWorldRef.TryGet(out NetworkObject itemWorldObj))
        {
            var itemWorldControl = itemWorldObj.GetComponent<ItemWorldControl>();
            itemWorldControl.targetTransform = null;
        }
    }
    
}
