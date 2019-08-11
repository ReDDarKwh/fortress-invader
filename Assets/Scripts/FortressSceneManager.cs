using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FortressSceneManager : MonoBehaviour
{
    public float timeBeforeGameover;

    public IntReactiveProperty score;

    public LevelChanger levelChanger;

    public GameOverInfo gameOverInfo;


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
        gameOverInfo.score = score.Value;

        if (gameOverInfo.score > gameOverInfo.highscore)
        {
            gameOverInfo.highscore = gameOverInfo.score;
        }

        gameOverInfo.type = type;
        levelChanger.FadeToLevel(levelChanger.levelToLoad);
    }

    public enum GameOverType
    {
        playerDied,
        allEnemiesKilled,
        timeout
    }
}
