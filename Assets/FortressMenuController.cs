﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Missions;
using UniRx;
using UnityEngine.UI;
using static UnityEngine.UI.Toggle;
using System.Linq;
using System;

public class FortressMenuController : MonoBehaviour
{
    public GameObject missionPrefab;
    public GameObject missionContentGameObject;
    public ToggleGroup missionToggleGroup;

    public void CloseMenu()
    {
        SharedSceneController.Instance.levelChanger.ExitMenu();

        var selectedIndex = SharedSceneController
            .Instance.missionController.missions.FindIndex(x => x.selected);

        // selected mission changed
        if (SharedSceneController.Instance.missionController.selectedMission.Value !=
            (selectedIndex == -1 ? null : SharedSceneController.Instance.missionController.missions.ElementAt(selectedIndex))
        )
        {
            SharedSceneController.Instance.missionController.lastSelectedMission.Value =
            SharedSceneController.Instance.missionController.selectedMission.Value;


            if (selectedIndex != -1)
            {

                var mission = SharedSceneController
                .Instance.missionController.missions.ElementAt(selectedIndex);

                SharedSceneController.Instance.missionController.selectedMission.Value = mission;

                // Move selected mission on top;
                SharedSceneController
                .Instance.missionController.missions.RemoveAt(selectedIndex);
                SharedSceneController.Instance.missionController.missions.Insert(0, mission);
            }
            else
            {
                SharedSceneController.Instance.missionController.selectedMission.Value = null;
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        var missions = SharedSceneController.Instance.missionController.missions;
        foreach (var mission in missions)
        {
            var missionUIElement = Instantiate(missionPrefab, missionContentGameObject.transform);
            var uiController = missionUIElement.GetComponent<MissionUIController>();
            uiController.mission = mission;
            uiController.selected.group = missionToggleGroup;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Menu"))
        {
            CloseMenu();
        }
    }
}
