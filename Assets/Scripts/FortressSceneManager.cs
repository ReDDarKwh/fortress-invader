using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;

public class FortressSceneManager : MonoBehaviour
{
    public float timeBeforeGameover;

    public FloatReactiveProperty chaos = new FloatReactiveProperty();


    [System.NonSerialized]
    public float timeSinceLevelLoad = 0;

    public void Start()
    {
        timeSinceLevelLoad = Time.unscaledTime;

        SharedSceneController.Instance.missionController.missions.Subscribe(missions =>
        {

            if (missions == null)
                return;

            foreach (var mission in missions)
            {
                mission.done.Subscribe(x =>
                {
                    if (x)
                    {
                        chaos.Value += mission.chaos;
                    }
                });
            }
        });
    }

    public void Update()
    {
        if (timeBeforeGameover - Time.timeSinceLevelLoad <= 0)
        {
            GameOver(GameOverType.timeout);
        }
    }

    public void GameOver(GameOverType type = GameOverType.timeout)
    {
        SharedSceneController.Instance.levelChanger.FadeToLevel(1);
    }

    public enum GameOverType
    {
        playerDied,
        allEnemiesKilled,
        timeout
    }
}
