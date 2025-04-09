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

    public Transform sun; 
    public Transform moon; 
    public float orbitRadius = 10f; 
    public Light2D globalLight;
    public Gradient gradient;

    public static event Action<ESeason> ChangeSeasonEvent;


    public static event Action<int> OnTimeIncrease;
    public int minutesToIncrease;

    private void Start()
    {
        
    }

    void MoveSunAndMoon()
    {
        float timeOfDay = (eStarus.DateTime.Hour * 60f + eStarus.DateTime.Minute) / (24f * 60f);

        float angle = timeOfDay * 360f * Mathf.Deg2Rad;

        sun.position = new Vector3(Mathf.Cos(angle) * orbitRadius, Mathf.Sin(angle) * orbitRadius, 0);

        moon.position = new Vector3(-Mathf.Cos(angle) * orbitRadius, -Mathf.Sin(angle) * orbitRadius, 0);
    }

    void UpdateSunAndMoonLight()
    {
        int hour = eStarus.DateTime.Hour;
        Light2D sunLight = sun.GetComponent<Light2D>();
        Light2D moonLight = moon.GetComponent<Light2D>();
        Light2D sunRLight = sun.GetComponentInChildren<Light2D>();
        Light2D moonRLight = moon.GetComponentInChildren<Light2D>();

        if (hour >= 6 && hour < 18)
        {
            sunLight.gameObject.SetActive(true);
            sunRLight.enabled = true;
            moonLight.gameObject.SetActive(false);
            moonRLight.enabled = false;
        }
        else
        {
            sunLight.gameObject.SetActive(false);
            sunRLight.enabled = false;
            moonLight.gameObject.SetActive(true);
            moonRLight.enabled = true;
        }
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
            ChangeColorDay();
            MoveSunAndMoon();
            UpdateSunAndMoonLight();
            if (ChangeSeason())
            {
                ChangeSeasonEvent?.Invoke(eStarus.SeasonStatus);
            }
            yield return new WaitForSeconds(1);
            eStarus.IncreaseDate(minutesToIncrease);
            OnTimeIncrease?.Invoke(minutesToIncrease);
        } while (true);
    }

    public void ChangeColorDay()
    {
        float timeOfDay = (eStarus.DateTime.Hour * 60f + eStarus.DateTime.Minute) / (24f * 60f);

        globalLight.color = gradient.Evaluate(timeOfDay);
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
