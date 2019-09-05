using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Missions;
using UnityEngine;
using Scripts.Spells;

public class SharedSceneController : MonoBehaviour
{
    public static SharedSceneController Instance;
    public MissionsController missionController;
    public LevelChanger levelChanger;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {




    }

    internal void OpenFortressMenu()
    {
        //missionController.ScanForMissions();
        levelChanger.LoadMenu(2);
    }
}
