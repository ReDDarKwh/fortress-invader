using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

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

    //     var nodes = mapGen.voronoi.SiteCoords();

    //     for (var i = 0; i < mapGen.numberOfPatches; i++)
    //     {
    //         Handles.Label(nodes[i], i.ToString());
    //     }
    // }
}