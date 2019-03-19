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


    void OnSceneGUI()
    {
        CityController mapGen = (CityController)target;


        foreach (var node in mapGen.heightSortedNodes)
        {
            Handles.Label(node.centerPoint, node.GetElevation().ToString());
        }


        // for (var i = 0; i < mapGen.mapGraph.nodesByCenterPosition.Values.Count; i++)
        // {

        //     Handles.Label(mapGen.mapGraph.nodesByCenterPosition.Values.ToList()[i].centerPoint, i.ToString());
        // }

    }
}