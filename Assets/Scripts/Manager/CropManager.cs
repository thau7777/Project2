
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : Singleton<CropManager>
{
    public Tilemap cropTilemap;

    private SerializableDictionary<Vector3Int, CropData> _plantedCrops = new SerializableDictionary<Vector3Int, CropData>();
    public SerializableDictionary<Vector3Int, CropData> PlantedCrops
    {
        get { return _plantedCrops; }
        set { _plantedCrops = value; }
    }

    private void OnEnable()
    {
        EnviromentalStatusManager.OnTimeIncrease += UpdateCropsGrowthTime;
    }

    private void OnDisable()
    {
        EnviromentalStatusManager.OnTimeIncrease -= UpdateCropsGrowthTime;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PlantCrop(bool isFarmGround, Vector3Int plantPosition, Item crop)
    {
        if (isFarmGround && !_plantedCrops.ContainsKey(plantPosition))
        {
            CropData newCrop = new CropData(crop.CropSetting.TimeToGrowth, crop.CropSetting.growthStages, crop.CropSetting.season, crop.CropSetting.cropProductName);

            _plantedCrops.Add(plantPosition, newCrop);
            cropTilemap.SetTile(plantPosition, newCrop.growthStages[0]);
            Debug.Log($"add crop tile at {plantPosition}");
        }
    }

    public bool Harverst(Vector3Int pos, Vector3 playerPos)
    {
        if (PlantedCrops.ContainsKey(pos) && PlantedCrops[pos].IsFullyGrown())
        {
            Item newCrop = ItemDatabase.Instance.items.Find(item => item.itemName == PlantedCrops[pos].cropName);
            int level = RatioPick.GetRandomLevel(PlantedCrops[pos].level, PlantedCrops[pos].ratio);
            newCrop.image = newCrop.cropLevelImage[level-1];
            RemoveCrop(pos);
            ItemWorld crop = new ItemWorld(System.Guid.NewGuid().ToString(), newCrop,1, playerPos);
            ItemWorldManager.Instance.AddItemWorld(crop);
            ItemWorldManager.Instance.SpawnItem();
            return true;
        }
        return false;
    }
    public void UpdateCropsGrowthTime(int minute)
    {
        foreach (var crop in _plantedCrops.ToList())
        {
            var cropInfo = crop.Value;
            cropInfo.isWatered = TileManager.Instance.WateredTiles.ContainsKey(crop.Key);
            if (!cropInfo.IsFullyGrown())
            {
                if (cropInfo.season != EnviromentalStatusManager.Instance.eStarus.SeasonStatus)
                {
                    Debug.Log(EnviromentalStatusManager.Instance.eStarus.SeasonStatus);
                    Debug.Log("crop dead");
                    cropTilemap.SetTile(crop.Key, cropInfo.growthStages[1]);
                }
                if (cropInfo.isWatered)
                {
                    cropInfo.GrowthTimeUpdate(minute);
                }
                if (cropInfo.needChangeStage)
                {
                    cropInfo.needChangeStage = false;
                    cropTilemap.SetTile(crop.Key, cropInfo.growthStages[cropInfo._currentStage]);
                }
            }
            else
            {
                Debug.Log("crop fully grew");
            }

        }
    }

    public void RemoveCrop(Vector3Int pos)
    {
        PlantedCrops.Remove(pos);
        cropTilemap.SetTile(pos,null);
    }

    public void LoadCrops(SerializableDictionary<Vector3Int, CropData> crops)
    {
        PlantedCrops = crops;
        foreach (var crop in PlantedCrops)
        {
            cropTilemap.SetTile(crop.Key, crop.Value.growthStages[crop.Value._currentStage]);
        }
    }
}
