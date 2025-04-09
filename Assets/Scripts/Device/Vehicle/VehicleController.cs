using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public float vehicleSpeed = 1f;
    public Vector2 movement;
    public Animator animator;
    [SerializeField]
    private PlayerController playerController;

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
            SetMovement(playerController.movement);

            animator.SetFloat("Speed", playerController.movement.magnitude);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (IsBeingRidden) return;
            PlayerController player = collision.GetComponent<PlayerController>();

            playerController = player;
            playerController.SetCurrentVehicle(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.IsRidingVehicle) return;

            playerController.ClearVehicle();
        }
    }

    public void SetRiding(bool riding)
    {
        IsBeingRidden = riding;

        if (riding)
        {
            playerController.transform.position = transform.position;
            playerController.GetComponent<Collider2D>().isTrigger = true;
            transform.localScale = playerController.transform.localScale;
            animator.SetFloat("Horizontal", Mathf.Abs(playerController.LastMovement.x));
            animator.SetFloat("Vertical", playerController.LastMovement.y);
            playerController.vehicleSpeed = vehicleSpeed;
        }
        else
        {
            playerController.transform.position = transform.position;
            playerController.GetComponent<Collider2D>().isTrigger = false;
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