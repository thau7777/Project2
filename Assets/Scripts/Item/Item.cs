
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObject/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    [Header("Only gameplay")]
    [Header("For Crops")]
    public Crop CropSetting;
    [Header("For Crops Products")]
    public Sprite[] cropLevelImage;

    [Header("For Tools")]
    public TileBase tile;
    public RuleTile ruleTile;
    public AnimatedTile animatedTile;
    public Tilemap tilemap;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public Sprite image;
    public string itemName;

    

}
[System.Serializable]
public class Crop
{
    public string cropProductName;
    public TileBase[] growthStages;
    public int TimeToGrowth;
    public ESeason season;
}
public enum ItemType
{
    Tile,
    Food,
    Tool,
    Material,
    Crop,
    Workbench
}

public enum ActionType
{
    Cook,
    Combine,
    Mine,
    Craft,
    PlaceTile
}
