using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HoedTileData
{

    [SerializeField] public int timeToRemoveTile;
    [SerializeField] public int removeTileCounter;
    [SerializeField] public bool hasSomethingOn;
    [SerializeField] public bool needRemove;

    public HoedTileData()
    {
        timeToRemoveTile = 4320;
        removeTileCounter = 0;
        hasSomethingOn = false;
        needRemove = false;
    }

    public void CheckTile(int minute)
    {
        if (!hasSomethingOn)
        {
            removeTileCounter += minute;
            if(removeTileCounter >= timeToRemoveTile)
            {
                needRemove = true;
            }
        }
        else
        {
            removeTileCounter = 0;
        }
    }
}
