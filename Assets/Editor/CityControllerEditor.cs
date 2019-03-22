using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CityController))]
public class CityControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        CityController mapGen = (CityController)target;

        if (DrawDefaultInspector())
        {
            //mapGen.generateNavMesh();
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.Init();
        }
    }


    // void OnSceneGUI()
    // {
    //     CityController mapGen = (CityController)target;


    // }
}