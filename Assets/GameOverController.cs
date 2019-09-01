using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FortressSceneManager;

public class GameOverController : MonoBehaviour
{

    public void Replay(){
        SharedSceneController.Instance.levelChanger.FadeToLevel(0);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
