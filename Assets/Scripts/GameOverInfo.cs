using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameOverInfo", menuName = "Util/GameOverInfo")]
public class GameOverInfo : ScriptableObject
{
    public int score;
    public int highscore;
    public FortressSceneManager.GameOverType type;
}
