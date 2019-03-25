using UnityEngine;
using System.Collections;
using UnityEditor;
using Scripts.AI;

[CustomEditor(typeof(Nav2D))]
public class MapGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Nav2D mapGen = (Nav2D)target;

        if (DrawDefaultInspector())
        {
            //mapGen.generateNavMesh();

        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateNavMesh();
        }



    }



}