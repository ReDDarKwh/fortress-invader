using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Loader : MonoBehaviour
{
    public UnityEngine.GameObject sceneManager;          //GameManager prefab to instantiate.

    void Awake()
    {
        Instantiate(sceneManager);
    }
}