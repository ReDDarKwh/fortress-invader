using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Missions;
using UniRx;
using UnityEngine.UI;
using static UnityEngine.UI.Toggle;
using System.Linq;

public class FortressMenuController : MonoBehaviour
{
    public GameObject missionPrefab;
    public ReactiveProperty<Mission> currentMission;
    public GameObject missionContentGameObject;
    public ToggleGroup missionToggleGroup;

    public void CloseMenu()
    {

        SharedSceneController.Instance.levelChanger.ExitMenu();

        //Order is important here. Menu exit reactivated objects from last scene.

        SharedSceneController.Instance.missionController.lastSelectedMission.Value =
        SharedSceneController.Instance.missionController.selectedMission.Value;

        SharedSceneController.Instance.missionController.selectedMission.Value = SharedSceneController
        .Instance.missionController.missions.FirstOrDefault(x => x.selected);
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
