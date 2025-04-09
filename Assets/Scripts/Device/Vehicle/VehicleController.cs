using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VehicleController : NetworkBehaviour
{
    public float vehicleSpeed = 1f;
    public Vector2 movement;
    public Animator animator;
    [SerializeField]
    private PlayerController _playerController;
    public PlayerController PlayerController
    {
        get { return _playerController; }
        set 
        { 
            _playerController = value; 
            if(_playerController == null) transform.SetParent(null);
            else transform.SetParent(value.transform);
        }
    }

    [SerializeField]
    private List<Collider2D> colliders;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        SetMovement(movement);
    }

    void Update()
    {
        if (IsBeingRidden)
        {
            SetMovement(PlayerController.LastMovement);

            animator.SetFloat("Speed", PlayerController.movement.magnitude);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (IsBeingRidden) return;
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.IsRidingVehicle) return;
            player.SetCurrentVehicle(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.IsRidingVehicle || IsBeingRidden) return;
            player.ClearVehicle();
        }
    }
    public void SetRiding(bool riding, PlayerController player = null)
    {
        IsBeingRidden = riding;

        if (riding)
        {
            PlayerController = player;
            PlayerController.GetComponent<Collider2D>().isTrigger = true;
            PlayerController.transform.position = transform.position;
            transform.localScale = PlayerController.transform.localScale;
            animator.SetFloat("Horizontal", Mathf.Abs(PlayerController.LastMovement.x));
            animator.SetFloat("Vertical", PlayerController.LastMovement.y);
            PlayerController.vehicleSpeed = vehicleSpeed;
        }
        else
        {

            PlayerController.transform.position = transform.position;
            PlayerController.GetComponent<Collider2D>().isTrigger = false;
            PlayerController = null;
            animator.SetFloat("Speed", 0);

        }
    }

    public void SetMovement(Vector2 movement)
    {
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", Mathf.Abs(movement.x));
            animator.SetFloat("Vertical", movement.y);
            SetCollision(movement);
        }
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