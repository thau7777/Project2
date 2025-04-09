using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSaveData
{
    [SerializeField] private SerializableDictionary<Vector3Int, HoedTileData> _hoedTiles;
    [SerializeField] private SerializableDictionary<Vector3Int, WateredTileData> _wateredTiles;
    [SerializeField] private SerializableDictionary<Vector3Int, CropData> _cropTiles;

    public SerializableDictionary<Vector3Int, HoedTileData> HoedTiles
    {
        get { return _hoedTiles; }
        set { _hoedTiles = value; }
    }

    public SerializableDictionary<Vector3Int, WateredTileData> WateredTiles
    {
        get { return _wateredTiles; }
        set { _wateredTiles = value; }
    }

    public SerializableDictionary<Vector3Int, CropData> CropTiles
    {
        get { return _cropTiles; }
        set { _cropTiles = value; }
    }

    public TileSaveData()
    {
        _hoedTiles = new SerializableDictionary<Vector3Int, HoedTileData>();
        _wateredTiles = new SerializableDictionary<Vector3Int, WateredTileData>();
        _cropTiles = new SerializableDictionary<Vector3Int, CropData>();
    }

    public void SetTiles(SerializableDictionary<Vector3Int, HoedTileData> hoedTiles,
        SerializableDictionary<Vector3Int, WateredTileData> wateredTiles,
        SerializableDictionary<Vector3Int, CropData> cropTiles)
    {
        HoedTiles = hoedTiles; 
        WateredTiles = wateredTiles;
        CropTiles = cropTiles;
        
    }

}
