using UnityEngine;
using System.Collections;
using UnityEditor;
using Scripts.AI;

[CustomEditor(typeof(BuildingController))]
public class BuildingEditor : Editor
{

    public override void OnInspectorGUI()
    {
        BuildingController b = (BuildingController)target;

        if (DrawDefaultInspector())
        {
            b.Init();
        }
    }

    public void OnSceneGUI()
    {
        BuildingController b = (BuildingController)target;

        if (b.WorldSpaceCorners == null)

            return;

        var style = new GUIStyle();
        style.fontSize = 12;
        style.normal.textColor = Color.white;

        foreach (var corner in b.WorldSpaceCorners)
        {

            Handles.Label(
               corner,
             corner.ToString(), style);
        }

        // radius
    }
}