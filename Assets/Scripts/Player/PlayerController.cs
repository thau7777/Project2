using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    //private Inventory inventory;
    [SerializeField] private InputReader _inputReader;
    public float walkSpeed = 1f;
    public float runSpeed = 1f; // :)) tuong de 1.5f
    public float vehicleSpeed;
    private string _currentState;
    public string CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }
    public string[] noTargetStates = { "Sword", "Axe", "Scythe" };
    public string[] toolsAndWeapon = { "Sword", "Axe", "Scythe", "WaterCan", "Pickaxe", "Shovel" };

    [SerializeField] private TileTargeter tileTargeter;

    [SerializeField]
    private bool _canMove = true;
    public bool CanMove
    {
        get { return _canMove; }
        set
        { 
            _canMove = value; 
        }
    }

    [SerializeField]
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get
        {
            return _currentSpeed = CanMove ? IsRidingVehicle ? vehicleSpeed : IsRunning ? runSpeed : walkSpeed : 0;
        }
    }

    public Vector2 movement;
    private Vector2 _lastMovement;
    public Vector2 LastMovement // Keep the last animation
    {
        get { return _lastMovement; }
        set
        {
            _lastMovement = value;
            animator.SetFloat("Horizontal", Mathf.Abs(_lastMovement.x));
            animator.SetFloat("Vertical", _lastMovement.y);
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
            if (_currentVehicle == null)
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

            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
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
    public bool IsRunning
    {
        get { return _isRuning; }
        private set 
        { 
            _isRuning = value;
            animator.SetBool("IsRunning", _isRuning);
        }
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
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private InventoryManagerSO _inventoryManagerSO;


    //Game Event
    [SerializeField] private GameEvent onPlayerLoad;
    [SerializeField] private GameEvent onPlayerSave;

    private void Awake()
    {
        _inventoryController = GetComponent<InventoryController>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

    }
    private void OnEnable()
    {
        _inputReader.playerActions.moveEvent += OnMove;
        _inputReader.playerActions.attackEvent += OnAttack;
        _inputReader.playerActions.interactEvent += OnInteract;
        _inputReader.playerActions.secondInteractEvent += OnSecondInteract;
        _inputReader.playerActions.runEvent += OnRun;
        _inventoryManagerSO.onChangedSelectedSlot += CheckAnimation;
    }

    private void OnDisable()
    {
        _inputReader.playerActions.moveEvent -= OnMove;
        _inputReader.playerActions.attackEvent -= OnAttack;
        _inputReader.playerActions.interactEvent -= OnInteract;
        _inputReader.playerActions.secondInteractEvent -= OnSecondInteract;
        _inputReader.playerActions.runEvent -= OnRun;
        _inventoryManagerSO.onChangedSelectedSlot -= CheckAnimation;
    }

    private void Start()
    {

        
        if (!IsOwner) enabled = false;
        else
        {
            virtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = transform;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        onPlayerLoad.Raise(this, null);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        onPlayerSave.Raise(this, null);
    }
    //void Update()
    //{
    //    CheckAnimation();
    //}

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * CurrentSpeed * Time.fixedDeltaTime);
    }
    private void OnSecondInteract()
    {
        if (CanRide && CurrentVehicle != null)
        {
            IsRidingVehicle = !IsRidingVehicle;
            if (IsRidingVehicle)
            {
                ChangeAnimationState("Idle");
                LastMovement = CurrentVehicle.VehicleLastMovement.Value;
                StartAllAction();
                RequestToRideVehicleServerRpc(
                    GetComponent<NetworkObject>(),
                    CurrentVehicle.GetComponent<NetworkObject>()
                );
            }
            else
            {
                RequestToUnRideVehicleServerRpc(
                    CurrentVehicle.GetComponent<NetworkObject>()
                );
            }
        }

        if (CanSleep)
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
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        if (!CanRun) return;
        if (context.phase == InputActionPhase.Started) IsRunning = true;
        else if (context.phase == InputActionPhase.Canceled) IsRunning = false;
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
        CurrentVehicle.GetComponent<SpriteRenderer>().color = Color.white;
        CurrentVehicle = null;
    }

    [ServerRpc]
    private void RequestToRideVehicleServerRpc(NetworkObjectReference playerRef, NetworkObjectReference vehicleRef, ServerRpcParams rpcParams = default)
    {
        if (playerRef.TryGet(out NetworkObject playerObj) && vehicleRef.TryGet(out NetworkObject vehicleObj))
        {
            var player = playerObj.GetComponent<PlayerController>();
            var vehicle = vehicleObj.GetComponent<VehicleController>();

            vehicle.SetRiding(true, playerRef);
            vehicle.transform.SetParent(playerObj.transform);

            FixVehicleLocalScaleClientRpc(vehicleRef, playerRef);
        }
    }


    [ClientRpc]
    private void FixVehicleLocalScaleClientRpc(NetworkObjectReference vehicleRef, NetworkObjectReference playerRef)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj) && playerRef.TryGet(out NetworkObject playerObj))
        {
            var player = playerObj.GetComponent<PlayerController>();
            var vehicle = vehicleObj.GetComponent<VehicleController>();
            if (vehicle.transform.localScale.x < 0) vehicle.transform.localScale = new Vector3(1, 1, 1);
            player.IsFacingRight = vehicle.IsFacingRight.Value;
        }

    }

    [ServerRpc]
    private void RequestToUnRideVehicleServerRpc(NetworkObjectReference vehicleRef)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj))
        {
            vehicleObj.transform.SetParent(null, true);
        }
        RequestToUnRideVehicleClientRpc(vehicleRef);
    }

    [ClientRpc]
    private void RequestToUnRideVehicleClientRpc(NetworkObjectReference vehicleRef)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj))
            vehicleObj.GetComponent<VehicleController>().SetRiding(false, GetComponent<NetworkObject>());
    }

    // ============== Movement ==================

    public void OnMove(Vector2 inputMovement)
    {
        movement = inputMovement.normalized;

        if (!CanMove) movement = Vector2.zero;
        if (movement != Vector2.zero) LastMovement = movement;

        animator.SetFloat("Speed", movement.magnitude);
        

        if (movement.x > 0 && !IsFacingRight) IsFacingRight = true;
        else if (movement.x < 0 && IsFacingRight) IsFacingRight = false;

        if (IsRidingVehicle)
        {
            SetCurrentVehicleMovementServerRpc(CurrentVehicle.GetComponent<NetworkObject>(), movement, IsFacingRight);
        }

    }


    [ServerRpc]
    private void SetCurrentVehicleMovementServerRpc(NetworkObjectReference vehicleRef, Vector2 movement, bool IsFacingRight)
    {
        if (vehicleRef.TryGet(out NetworkObject vehicleObj))
        {
            var vehicle = vehicleObj.GetComponent<VehicleController>();
            vehicle.SetMovement(movement);
            if (movement != Vector2.zero)
                vehicle.IsFacingRight.Value = IsFacingRight;
        }
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
    
 

    // ===================== Animation =====================

    
    public void CheckAnimation()
    {
        if (!CanAttack || IsRidingVehicle) return;
        Item item = _inventoryManagerSO.GetCurrentItem();
        _itemOnHand.gameObject.SetActive(false);
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
        else ChangeAnimationState(AnimationStrings.idle);
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
        Item item = _inventoryManagerSO.GetCurrentItem();
        Debug.Log(item.name);
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

    private void OnAttack()
    {
        if (!IsRidingVehicle && IsHoldingItem && CanAttack && Input.GetMouseButton(0))
        {
            UseCurrentItem();
        }
    }

    private void OnInteract()
    {
        if (!IsRidingVehicle && CanAttack && Input.GetMouseButton(1))
        {
            if (tileTargeter.CheckHarverst(transform.position))
            {
                animator.SetTrigger(AnimationStrings.pickup);
                _itemOnHand.gameObject.SetActive(false);

            }
        }
    }
    // Load & Save
    public void StartToLoad(GameData gameData)
    {
        player = gameData.PlayerData;
        transform.position = player.Position;
        _inventoryController.StartToLoad(gameData);
    }

    public void StartToSave(ref GameData gameData)
    {
        player.SetPosition(transform.position);
        gameData.SetPlayerData(player);
        _inventoryController.StartToSave(ref gameData);
    }
}