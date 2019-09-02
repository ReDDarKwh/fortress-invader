using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Scripts.Missions;

[CustomEditor(typeof(MissionsController))]
public class MissionControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MissionsController control = (MissionsController)target;

        if (DrawDefaultInspector())
        {
            //mapGen.generateNavMesh();
        }

        if (GUILayout.Button("Generate"))
        {
            control.ScanForMissions();
        }
    }


}