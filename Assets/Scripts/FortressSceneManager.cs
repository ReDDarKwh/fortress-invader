using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FortressSceneManager : MonoBehaviour
{
    public float timeBeforeGameover;

    public IntReactiveProperty score;


    [System.NonSerialized]
    public float timeSinceLevelLoad = 0;

    public void Start()
    {
        timeSinceLevelLoad = Time.unscaledTime;
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
