using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class EnviromentalStatusManager : Singleton<EnviromentalStatusManager>, IDataPersistence
{
    public EnvironmentalStatus eStarus;
    public UI_EnviromentStatus statusUI;

    public static event Action<ESeason> ChangeSeasonEvent;

    public static event Action<int> OnTimeIncrease;
    public int minutesToIncrease;

    private void Start()
    {
        
    }

    public bool ChangeSeason()
    {
        switch (eStarus.DateTime.Month, eStarus.DateTime.Day, eStarus.DateTime.Hour, eStarus.DateTime.Minute)
        {
            case (1, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Spring);
                    return true;
                }
            case (4, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Summer);
                    return true;
                }
            case (7, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Autumn);
                    return true;
                }
            case (10, 1, 0, 0):
                {
                    eStarus.SetSeasonStatus(ESeason.Winter);
                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }

    IEnumerator WaitToIncreaseDay()
    {
        do
        {
            statusUI.UpdateDateText(eStarus.DateTime);
            if (ChangeSeason())
            {
                ChangeSeasonEvent?.Invoke(eStarus.SeasonStatus);
            }
            yield return new WaitForSeconds(1);
            eStarus.IncreaseDate(minutesToIncrease);
            OnTimeIncrease?.Invoke(minutesToIncrease);
        } while (true);
    }

    public void LoadData(GameData gameData)
    {
        eStarus = gameData.EnviromentData;
        StartCoroutine(WaitToIncreaseDay());
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.SetSeason(eStarus);
    }
}
