using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TileTargeter : NetworkBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField]
    private List<Tilemap> _tilemaps; 
    public List<Tilemap> Tilemaps
    {
        get { return _tilemaps; }
        set { _tilemaps = value; }
    }

    [SerializeField]
    private Tilemap _targetTilemap;
    public Tilemap TargetTilemap
    {
        get { return _targetTilemap; }
        set { _targetTilemap = value; }
    }

    [Header("TARGET TILE SETTINGS")]
    [SerializeField]
    private AnimatedTile _targetTile;
    public AnimatedTile TargetTile
    {
        get { return _targetTile; }
        set { _targetTile = value; }
    }

    [SerializeField]
    private int TargetRange = 1;

    private Vector3 _mouseWorldPosition;
    private Vector3Int _previousTilePos;
    private Vector3Int _mouseTilePosition;
    private Vector3Int _playerTilePosition;
    private Vector3Int _clampedTilePosition;
    private Vector3Int _lockedTilePosition;

    [SerializeField] private List<Tilemap> tilemapCheck = new List<Tilemap>();
    [Header("HOE ON TILES SETTINGS")]
    [SerializeField] private bool _canHoe = false;
    public bool CanHoe
    {
        get { return _canHoe; }
        set { _canHoe = value; }
    }

    [SerializeField] private bool _lockedCanHoe = false;
    public bool LockedCanHoe
    {
        get { return _lockedCanHoe; }
        set { _lockedCanHoe = value; }
    }


    [Header("WATER ON TILES SETTINGS")]
    [SerializeField] private bool _canWater = false;
    public bool CanWater
    {
        get { return _canWater; }
        set { _canWater = value; }
    }

    [SerializeField] private bool _lockedCanWater = false;
    public bool LockedCanWater
    {
        get { return _lockedCanWater; }
        set { _lockedCanWater = value; }
    }

    [Header("PLANT ON TILES SETTINGS")]
    [SerializeField] private bool _canPlantGround = false;
    public bool CanPlantGround
    {
        get { return _canPlantGround; }
        set { _canPlantGround = value; }
    }

    private void Awake()
    {
        GetAllTilemaps();
        TargetTilemap = Tilemaps.LastOrDefault();
    }
    void Update()
    {
        if(!IsOwner) return;
        GetTargetTile();
    }
    void GetAllTilemaps()
    {
        GameObject gridObject = GameObject.Find("Grid");

        if (gridObject == null)
        {
            Debug.LogError("No Grid found in the scene!");
            return;
        }

        Tilemaps.Clear();
        Tilemap[] foundTilemaps = gridObject.GetComponentsInChildren<Tilemap>();

        Tilemaps.AddRange(foundTilemaps);

    }
    void GetTargetTile()
    {

        // Get mouse position in world coordinates
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mouseWorldPosition.z = 0; // Ensure it's on the correct plane

        // Convert mouse world position to tile position
        _mouseTilePosition = TargetTilemap.WorldToCell(_mouseWorldPosition);

        // Get player position in tile coordinates
        _playerTilePosition = TargetTilemap.WorldToCell(transform.position);

        // Ensure the highlight stays within 1 tile range of the player
        _clampedTilePosition = new Vector3Int(
            Mathf.Clamp(_mouseTilePosition.x, _playerTilePosition.x - TargetRange, _playerTilePosition.x + TargetRange),
            Mathf.Clamp(_mouseTilePosition.y, _playerTilePosition.y - TargetRange, _playerTilePosition.y + TargetRange),
            _mouseTilePosition.z
        );

        // Only update if tile position has changed
        if (_clampedTilePosition != _previousTilePos)
        {
            RefreshTilemapCheck(playerController.noTargetStates.Contains(playerController.CurrentState) ? false : true);
        }

    }

    public void RefreshTilemapCheck(bool showTarget)
    {
        tilemapCheck.Clear();
        TargetTilemap.SetTile(_previousTilePos, null); // Remove previous highlight

        foreach (Tilemap tilemap in Tilemaps)
        {
            if (tilemap.HasTile(_clampedTilePosition)) 
            {
                tilemapCheck.Add(tilemap);
            }
        }

        if (showTarget)
        {
            TargetTilemap.SetTile(_clampedTilePosition, TargetTile); 
        }
        _previousTilePos = _clampedTilePosition;

        // Check if tile is valid to do something
        CanHoe = (tilemapCheck.Count == 1 && tilemapCheck[0].name == "Walkfront");
        CanWater = TileManager.Instance.HoedTiles.ContainsKey(_clampedTilePosition) && !TileManager.Instance.WateredTiles.ContainsKey(_clampedTilePosition);
        CanPlantGround = (tilemapCheck[tilemapCheck.Count - 1].name == "FarmGround" || tilemapCheck[tilemapCheck.Count - 1].name == "WateredGround");
    }

    public bool CheckHarverst(Vector3 playerPos)
    {
        return CropManager.Instance.Harverst(_clampedTilePosition, playerPos);
    }
    public void UseTool(bool changeFacingDirection)
    {
        if (changeFacingDirection)
        {
            ChangePlayerFacingDirection();
            RefreshTilemapCheck(changeFacingDirection);
            LockClampedPosition();
            LockedCanHoe = CanHoe;
            LockedCanWater = CanWater;
        }

    }

    public void LockClampedPosition()
    {
        _lockedTilePosition = _clampedTilePosition;
    }
    public void PlaceTile(Item item)
    {
        switch (item.itemName)
        {
            default:
                {
                    break;
                }
            case "Hoe":
                {
                    UseHoe(item);
                    break;
                }
            case "WaterCan":
                {
                    UseWaterCan(item);
                    break;
                }
        }
    }
    private void ChangePlayerFacingDirection()
    {
        if (_clampedTilePosition.x < _playerTilePosition.x)
        {
            playerController.LastMovement = Vector2.left;
            playerController.IsFacingRight = false;
        }
        else if (_clampedTilePosition.x > _playerTilePosition.x)
        {
            playerController.LastMovement = Vector2.right;
            playerController.IsFacingRight = true;
        }

        if (_clampedTilePosition.y > _playerTilePosition.y)
        {
            playerController.LastMovement = Vector2.up;
        }
        else if (_clampedTilePosition.y < _playerTilePosition.y)
        {
            playerController.LastMovement = Vector2.down;
        }
    }
    private void UseHoe(Item item)
    {

        if (LockedCanHoe)
        {
            Tilemap targetTilemap = null;
            foreach (Tilemap tilemap in Tilemaps)
            {
                if(tilemap.name == item.tilemap.name)
                {
                    targetTilemap = tilemap;
                    break;
                }
                    
                
            }
            if (!TileManager.Instance.HoedTiles.ContainsKey(_lockedTilePosition))
            {
                targetTilemap.SetTile(_lockedTilePosition, item.ruleTile);
                TileManager.Instance.AddHoedTile(_lockedTilePosition);
            }
            else
            {
                Debug.Log("Already hoe");
            }


        }
        else Debug.Log("Cant Hoe here");
    }

    private void UseWaterCan(Item item)
    {
        if (LockedCanWater)
        {
            Tilemap targetTilemap = null;
            foreach (Tilemap tilemap in Tilemaps)
            {
                if (tilemap.name == item.tilemap.name)
                {
                    targetTilemap = tilemap;
                    break;
                }
                    
            }
            if (!TileManager.Instance.WateredTiles.ContainsKey(_lockedTilePosition))
            {
                targetTilemap.SetTile(_lockedTilePosition, item.ruleTile);
                TileManager.Instance.AddWateredTile(_lockedTilePosition);
            }
            else
            {
                Debug.Log("Already water");
            }
                
        }
        else
        {
            Debug.Log("Cant water here");
        }
    }

    public void SetTile(Item item)
    {
        switch(item.type)
        {
            default:
                break;

            case ItemType.Crop:
                {
                    GameObject.Find("CropManager").GetComponent<CropManager>().PlantCrop(CanPlantGround,_clampedTilePosition,item);
                    break;
                }        
        }
    }


}
