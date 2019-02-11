using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortressSceneManager : MonoBehaviour
{

    // Use this for initialization
    public static FortressSceneManager instance;
    public UnityEngine.GameObject player;
    public Camera cam;

    void Awake()
    {

        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);




        player = UnityEngine.GameObject.FindGameObjectWithTag("Player");

    }


}
