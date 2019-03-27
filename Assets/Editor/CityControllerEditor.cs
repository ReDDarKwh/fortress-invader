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

        if (mapGen.cityModel != null)
        {
            foreach (var patch in mapGen.cityModel.patches)
            {
                Handles.Label(mapGen.transform.TransformPoint(patch.shape.center.vec), patch.ward.getLabel());
            }
            Handles.DrawWireDisc(mapGen.transform.position, Vector3.forward, mapGen.cityModel.cityRadius * 10);
        }


    }
}