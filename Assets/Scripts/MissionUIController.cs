using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Missions;
using static UnityEngine.UI.Toggle;

public class MissionUIController : MonoBehaviour
{
    public Text missionName;
    public Text desc;
    public Text difficulty;
    public Text chaos;
    public Toggle selected;
    public Mission mission;
    public CanvasGroup canvasGroup;

    public void SetSelected(bool selected)
    {
        mission.selected = selected;

    }

    // Start is called before the first frame update
    void Start()
    {
        missionName.text = mission.missionName;
        desc.text = mission.desc;
        chaos.text = $"{mission.chaos.ToString("0.00")}%";
        difficulty.text = "<b>Difficulty : </b>";
        selected.isOn = mission.selected;

        canvasGroup.alpha = mission.done.Value == true ? 0.5f : 1;


        switch (mission.difficulty)
        {
            case MissionDifficulty.VeryEasy:
                difficulty.text += "VERY EASY";
                break;
            case MissionDifficulty.Easy:
                difficulty.text += "EASY";
                break;
            case MissionDifficulty.Medium:
                difficulty.text += "MEDIUM";
                break;
            case MissionDifficulty.Hard:
                difficulty.text += "HARD";
                break;
            case MissionDifficulty.Impossible:
                difficulty.text += "IMPOSSIBLE";
                break;
            case MissionDifficulty.Insane:
                difficulty.text += "INSANE";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
