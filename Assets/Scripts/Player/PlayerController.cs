using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDataPersistence
{
    public float walkSpeed = 1f;
    public float runSpeed = 1f;
    public float vehicleSpeed;
    private string _currentState;
    public string CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    public List<string> noTargetStates = new List<string> { "Sword", "Axe", "Scythe", "Pickaxe" };

    [SerializeField] private TileTargeter tileTargeter;

    [SerializeField]
    private bool _canMove = true;
    public bool CanMove
    {
        get { return _canMove; }
        set 
        { _canMove = value; }
    }

    [SerializeField]
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get
        {
            return _currentSpeed = CanMove ? IsRidingVehicle ? vehicleSpeed : IsRuning ? runSpeed : walkSpeed : 0;
        }
    }

    public Vector2 movement;
    private Vector2 lastMovement;
    public Vector2 LastMovement
    {
        get { return lastMovement; }
        set
        {
            lastMovement = value;
            animator.SetFloat("Horizontal", Mathf.Abs(lastMovement.x));
            animator.SetFloat("Vertical", lastMovement.y);
        }

    }

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    private Player player;
    
    [SerializeField]
    private VehicleController _currentVehicle;
    public VehicleController CurrentVehicle
    {
        get { return _currentVehicle; }
        private set 
        { 
            _currentVehicle = value; 
            if(_currentVehicle == null)
            {
                HadTarget = false;
                CanRide = false;
            }
            else
            {
                HadTarget = true;
                CanRide = true;
            }
        }
    }

    [SerializeField]
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {
            if (_isFacingRight != value)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }

            _isFacingRight = value;
        }
    }

    [SerializeField]
    private bool _canRun = true;
    public bool CanRun
    {
        get { return _canRun; }
        private set { _canRun = value; }
    }

    [SerializeField]
    private bool _isRuning = false;
    public bool IsRuning
    {
        get { return _isRuning; }
        private set { _isRuning = value; }
    }

    [SerializeField]
    private bool _canRide = false;
    public bool CanRide
    {
        get { return _canRide; }
        private set { _canRide = value; }
    }



    [SerializeField]
    private bool _isRidingVehicle = false;
    public bool IsRidingVehicle
    {
        get { return _isRidingVehicle; }
        private set
        {
            _isRidingVehicle = value;
            if (value)
            {
                if (CurrentVehicle.tag == "Bicycle")
                    animator.SetBool("UseDevice", true);

                else if (CurrentVehicle.tag == "Horse")
                    animator.SetBool("UseHorse", true);
            }
            else
            {
                animator.SetBool("UseDevice", false);
                animator.SetBool("UseHorse", false);
            }

        }
    }

    [SerializeField]
    private bool _isHoldingItem = false;
    public bool IsHoldingItem
    {
        get { return _isHoldingItem; }
        private set
        {
            _isHoldingItem = value;
        }
    }

    [SerializeField]
    private bool _canAttack = true;
    public bool CanAttack
    {
        get { return _canAttack; }
        private set { _canAttack = value; }
    }

    [SerializeField]
    private bool _canSleep = false;
    public bool CanSleep
    {
        get { return _canSleep; }
        private set { _canSleep = value; }
    }

    [SerializeField]
    private bool _isSleeping = false;
    public bool IsSleeping
    {
        get { return _isSleeping; }
        private set { _isSleeping = value; }
    }

    [SerializeField]
    private BedScript _currentBed;
    public BedScript CurrentBed
    {
        get { return _currentBed; }
        private set
        {
            _currentBed = value;
            if (_currentBed == null)
            {
                HadTarget = false;
                CanSleep = false;
            }
            else
            {
                HadTarget = true;
                CanSleep = true;
            }
        }
    }

    [SerializeField]
    private bool _hadTarget;
    public bool HadTarget
    {
        get { return _hadTarget; }
        private set { _hadTarget = value; }
    }

    [SerializeField]
    private ItemOnHand _itemOnHand;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if(!IsOwner) return;
        if (CanRun && Input.GetKey(KeyCode.LeftShift)) IsRuning = true;
        else IsRuning = false;

        OnMove();

        if (Input.GetKeyDown(KeyCode.E) && CanRide)
        {
            IsRidingVehicle = !IsRidingVehicle;

            if (IsRidingVehicle)
            {
                ChangeAnimationState("Idle");
                StartAllAction();
                if(!IsServer)
                RequestStartRidingServerRpc();
                else CurrentVehicle.SetRiding(true, this);
            }
            else
            {
                if (!IsServer)
                    RequestStopRidingServerRpc();
                else CurrentVehicle.SetRiding(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && CanSleep)
        {
            if (IsSleeping)
            {
                StartAllAction();
                IsSleeping = !IsSleeping;
                CurrentBed.SetSleep(IsSleeping);
                animator.SetBool(AnimationStrings.isSleep, false);
            }
            else
            {
                StopAllAction();
                animator.SetBool(AnimationStrings.isSleep, true);
                
                IsSleeping = !IsSleeping;
                CurrentBed.SetSleep(IsSleeping);
            }
        }


        if(!IsRidingVehicle)
            CheckAnimation();
     
        if (!IsRidingVehicle && IsHoldingItem && CanAttack && Input.GetMouseButton(0))
        {
            UseCurrentItem();
        }

        if(!IsRidingVehicle && CanAttack && Input.GetMouseButton(1))
        {
            if (tileTargeter.CheckHarverst(transform.position))
            {
                animator.SetTrigger(AnimationStrings.pickup);
                _itemOnHand.gameObject.SetActive(false);

            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * CurrentSpeed * Time.fixedDeltaTime);
    }

    // ============== Bed Stuff =============
    public void SetCurrentBed(BedScript bed)
    {
        if (HadTarget) return;
        CurrentBed = bed;
        CurrentBed.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ClearBed()
    {
        if (CurrentBed == null) return;
        CurrentBed.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentBed = null;

    }

    // ============= Vehicle ================
    public void SetCurrentVehicle(VehicleController vehicle)
    {
        if (HadTarget) return;
        CurrentVehicle = vehicle;
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ClearVehicle()
    {
        if( CurrentVehicle == null) return;
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentVehicle = null;
    }




    [ServerRpc]
    void RequestStartRidingServerRpc()
    {
        CurrentVehicle.SetRiding(true, this);
    }

    [ServerRpc]
    void RequestStopRidingServerRpc()
    {
        CurrentVehicle.SetRiding(false);
    }







    // ============== Movement ==================
    public void SetFacing()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void OnMove()
    {
        if (!CanMove) return;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;

        if (movement != Vector2.zero) LastMovement = movement;

        

        animator.SetFloat("Speed", movement.magnitude);
        animator.SetBool("IsRunning", Input.GetKey(KeyCode.LeftShift));

        if (movement.x > 0 && !IsFacingRight) IsFacingRight = true;
        else if (movement.x < 0 && IsFacingRight) IsFacingRight = false;
    }

    private void StopAllAction()
    {
        CanMove = false;
        CanAttack = false;
    }

    private void StartAllAction()
    {
        CanMove = true;
        CanAttack = true;
    }
    // =================== Item ======================
    public void PickupItem(ItemWorld item)
    {
        bool result = InventoryManager.Instance.AddItemToInventory(item);
        if (result == true)
        {
            Debug.Log("Item added");
            IsHoldingItem = true;
        }
        else
        {
            Debug.Log("Item not added");
        }
    }

    public Item GetSelectedItem()
    {
        Item receivedItem = InventoryManager.Instance.GetSelectedItem(false);
        if (receivedItem != null)
        {
            return receivedItem;
        }

        return null;
    }

    public void UseSelectedItem()
    {
        Item receivedItem = InventoryManager.Instance.GetSelectedItem(true);
        if (receivedItem != null)
        {
            Debug.Log("Used item: " + receivedItem);
        }
        else
        {
            Debug.Log("Not item used");
        }
    }

    // ===================== Animation =====================
    public void CheckAnimation()
    {
        if (!CanAttack) return;
        _itemOnHand.gameObject.SetActive(false);
        Item item = GetSelectedItem();
        if (item != null)
        {
            IsHoldingItem = true;
        }
        else
        {
            IsHoldingItem = false;
        }

        if (IsHoldingItem)
        {
            
            switch (item.type)
            {
                default:
                    {
                        ChangeAnimationState("Idle");
                        
                        break;
                    }
                case ItemType.Tool:
                    {

                        ChangeAnimationState(item.name);
                        
                        break;
                    }
                case ItemType.Crop:
                case ItemType.Food:
                    {
                        _itemOnHand.gameObject.SetActive(true);
                        ChangeAnimationState("Pickup_idle");
                        _itemOnHand.SetItemSprite(item.image);
                        break;
                    }
            }
        }

        if (!IsHoldingItem || InventoryManager.Instance.GetSelectedItem(false) == null)
        {
            ChangeAnimationState(AnimationStrings.idle);
        }
    }

    private void ChangeAnimationState(string newState)
    {
        if (CurrentState == newState) return;
        animator.Play(newState);
        CurrentState = newState;
        tileTargeter.RefreshTilemapCheck(!noTargetStates.Contains(newState));
        
    }


    private void UseCurrentItem()
    {
        Item item = InventoryManager.Instance.GetSelectedItem(false);
        switch (item.type)
        {
            default:
                {
                    break;
                }
            case ItemType.Tool:
                {
                    animator.SetTrigger("Attack");
                    tileTargeter.UseTool(!noTargetStates.Contains(CurrentState));
                    break;
                }
            case ItemType.Crop:
                {
                    tileTargeter.SetTile(item);
                    break;
                }
        }   
    }
    // Load & Save
    public void LoadData(GameData gameData)
    {
        player = gameData.PlayerData;
        transform.position = player.Position;
    }

    public void SaveData(ref GameData gameData)
    {
        player.SetPosition(transform.position); 
        gameData.SetPlayerData(player);
    }
}