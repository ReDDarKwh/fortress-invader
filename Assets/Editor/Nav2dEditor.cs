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

        // if (GUILayout.Button("Generate"))
        // {
        //     mapGen.GenerateNavMesh();
        // }



    }

    // void OnSceneGUI()
    // {
    //     Nav2D nav = (Nav2D)target;

    //     foreach (var node in nav.nodes)
    //     {

    //         if (node.travelCost > 0)
    //             Handles.Label(node.worldPos, node.travelCost.ToString());
    //     }

    //     // if (mapGen.cityModel != null)
    //     // {
    //     //     foreach (var patch in mapGen.cityModel.patches)
    //     //     {
    //     //         Handles.Label(mapGen.transform.TransformPoint(patch.shape.center.vec), patch.ward.getLabel());
    //     //     }
    //     //     Handles.DrawWireDisc(mapGen.transform.position, Vector3.forward, mapGen.cityModel.cityRadius * 10);
    //     // }


    // }



}