using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FortressSceneManager;

public class GameOverController : MonoBehaviour
{

    public GameOverInfo gameOverInfo;
    public Text gameOverTypeText;
    public Text scoreText;
    public Text highscoreText;

    // Start is called before the first frame update
    void Start()
    {
        switch (gameOverInfo.type)
        {
            case GameOverType.allEnemiesKilled:
                gameOverTypeText.text = "All enemies killed!";
                break;
            case GameOverType.playerDied:
                gameOverTypeText.text = "Your invader died";
                break;
            case GameOverType.timeout:
                gameOverTypeText.text = "Timeout";
                break;
        }

        scoreText.text = "Score: " + gameOverInfo.score.ToString();
        highscoreText.text = "Highscore: " + gameOverInfo.highscore.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
