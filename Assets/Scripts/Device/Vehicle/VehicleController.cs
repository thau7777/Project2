using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static VehicleController;

public class VehicleController : NetworkBehaviour
{
    public float vehicleSpeed = 1f;
    public Vector2 DefaultFacingDirection;
    public NetworkVariable<Vector2> VehicleLastMovement = new NetworkVariable<Vector2>(
        writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);
    public Animator animator;
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private List<Collider2D> colliders;



    public NetworkVariable<bool> IsFacingRight = new NetworkVariable<bool>(true,
        writePerm: NetworkVariableWritePermission.Server, readPerm: NetworkVariableReadPermission.Everyone);


    [SerializeField]
    private bool _isBeingRidden = false;
    public bool IsBeingRidden
    {
        get { return _isBeingRidden; }
        private set
        {
            _isBeingRidden = value;
            animator.Play("Running");
            animator.SetBool("IsRiding", value);
        }
    }



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        VehicleLastMovement.OnValueChanged += SetFacingDirectionByAnimator;
        VehicleLastMovement.Value = DefaultFacingDirection;
    }
    private void OnDisable()
    {
        VehicleLastMovement.OnValueChanged -= SetFacingDirectionByAnimator;
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsBeingRidden) return;
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.SetCurrentVehicle(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.CurrentVehicle == this && !player.IsRidingVehicle)
            {
                player.ClearVehicle();
            }
        }
    }

    public void SetRiding(bool riding, NetworkObjectReference playerRef)
    {
        IsBeingRidden = riding;

        if (riding)
        {
            SetPlayerOnRideVehicleClientRpc(playerRef);
        }
        else
        {
            playerController.GetComponent<Collider2D>().isTrigger = false;
            playerController = null;
            animator.SetFloat("Speed", 0);
        }
    }

    [ClientRpc]
    private void SetPlayerOnRideVehicleClientRpc(NetworkObjectReference playerRef)
    {
        if(playerRef.TryGet(out NetworkObject playerObj))
        {
            playerController = playerObj.GetComponent<PlayerController>();
            playerController.transform.position = transform.position;
            playerController.GetComponent<Collider2D>().isTrigger = true;
            playerController.vehicleSpeed = vehicleSpeed;
        }
        
    }


    public void SetMovement(Vector2 movement)
    {
        animator.SetFloat("Speed", movement.magnitude);

        if(movement != Vector2.zero) VehicleLastMovement.Value = movement;

    }

    private void SetFacingDirectionByAnimator(Vector2 oldValue, Vector2 newValue)
    {
        animator.SetFloat("Horizontal", Mathf.Abs(newValue.x));
        animator.SetFloat("Vertical", newValue.y);
        SetCollision(newValue);
    }


    public void SetCollision(Vector2 movement)
    {
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        switch (movement.x, movement.y)
        {
            case (1, 0):
                {
                    colliders[1].enabled = true;
                    break;
                }
            case (0, 1):
                {
                    colliders[2].enabled = true;
                    break;
                }
            case (0, -1):
                {
                    colliders[0].enabled = true;
                    break;
                }
            default:
                {
                    colliders[1].enabled = true;
                    break;
                }
        }
    }

}