using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>, IDataPersistence
{
    [SerializeField]
    private Tilemap _hoedTilemap;
    [SerializeField]
    private Tilemap _wateredTilemap;

    [SerializeField]
    private RuleTile _hoedRuleTile;
    [SerializeField]
    private RuleTile _wateredRuleTile;
    private SerializableDictionary<Vector3Int, HoedTileData> _hoedTiles = new SerializableDictionary<Vector3Int, HoedTileData>();
    public SerializableDictionary<Vector3Int, HoedTileData> HoedTiles
    {
        get { return _hoedTiles; }
        set { _hoedTiles = value; }
    }
   
    private SerializableDictionary<Vector3Int, WateredTileData> _wateredTiles = new SerializableDictionary<Vector3Int, WateredTileData>();
    public SerializableDictionary<Vector3Int, WateredTileData> WateredTiles
    {
        get { return _wateredTiles; }
        set { _wateredTiles = value; }
    }

    private TileSaveData _tileSaveData = new TileSaveData();
    private void OnEnable()
    {
        EnviromentalStatusManager.OnTimeIncrease += UpdateAllTileStatus;
    }

    private void OnDisable()
    {
        EnviromentalStatusManager.OnTimeIncrease -= UpdateAllTileStatus;
    }

    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAllTileStatus(int minute)
    {
        foreach (var hoedTile in HoedTiles.ToList())
        {
            Vector3Int hoedPosition = hoedTile.Key;
            HoedTileData hoedTileData = hoedTile.Value;

            if (WateredTiles.ContainsKey(hoedPosition) || CropManager.Instance.PlantedCrops.ContainsKey(hoedPosition))
            {
                hoedTileData.hasSomethingOn = true;
            }
            hoedTileData.CheckTile(minute);
            if (hoedTileData.needRemove)
            {
                RemoveHoedTile(hoedPosition);
            }
             
        }

        foreach (var wateredTile in WateredTiles.ToList())
        {
            Vector3Int wateredPosition = wateredTile.Key;
            WateredTileData wateredTileData = wateredTile.Value;


            wateredTileData.CheckTile(minute);
            if (wateredTileData.needRemove)
            {
                RemoveWateredTile(wateredPosition);
            }

        }
    }

    public void AddHoedTile(Vector3Int tilePos)
    {
        HoedTileData newHoedTile = new HoedTileData();
        _hoedTiles.Add(tilePos, newHoedTile);

        Debug.Log($"add hoed tile at {tilePos}");
    }

    public void RemoveHoedTile(Vector3Int tilePos)
    {
        _hoedTilemap.SetTile(tilePos, null);
        HoedTiles.Remove(tilePos);
        if(WateredTiles.ContainsKey(tilePos)) RemoveWateredTile(tilePos);

        Debug.Log($"removed hoed tile at {tilePos}");
    }


    public void AddWateredTile(Vector3Int tilePos)
    {
        WateredTileData newWateredTile = new WateredTileData();
        _wateredTiles.Add(tilePos, newWateredTile);
        Debug.Log($"add watered tile at {tilePos}");
    }

    public void RemoveWateredTile(Vector3Int tilePos)
    {
        _wateredTilemap.SetTile(tilePos, null);
        WateredTiles.Remove(tilePos);

        Debug.Log($"removed watered tile at {tilePos}");
    }

    public void LoadData(GameData data)
    {
        HoedTiles = data.TileSaveData.HoedTiles;
        WateredTiles = data.TileSaveData.WateredTiles;

        StartCoroutine(ApplyTileUpdates(data));
    }
    private IEnumerator ApplyTileUpdates(GameData data)
    {
        yield return new WaitForEndOfFrame(); // Wait until rendering is done

        foreach (var hoedTile in HoedTiles)
        {
            _hoedTilemap.SetTile(hoedTile.Key, _hoedRuleTile);
        }

        foreach (var wateredTile in WateredTiles)
        {
            _wateredTilemap.SetTile(wateredTile.Key, _wateredRuleTile);
        }

        CropManager.Instance.LoadCrops(data.TileSaveData.CropTiles);
    }
    public void SaveData(ref GameData data)
    {
        _tileSaveData.SetTiles(HoedTiles, WateredTiles, CropManager.Instance.PlantedCrops);
        data.SetTiles(_tileSaveData);
    }
}
