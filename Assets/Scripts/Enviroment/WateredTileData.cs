using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WateredTileData
{
    [SerializeField] public int timeToRemoveTile;
    [SerializeField] public int removeTileCounter;
    [SerializeField] public bool needRemove;

    public WateredTileData()
    {
        timeToRemoveTile = 1440;
        removeTileCounter = 0;
        needRemove = false;
    }

    public void CheckTile(int minute)
    {
        removeTileCounter += minute;
        if (removeTileCounter >= timeToRemoveTile)
        {
            needRemove = true;
        }
    }
}
