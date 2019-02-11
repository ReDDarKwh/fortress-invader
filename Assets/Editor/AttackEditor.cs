using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AttackController))]
public class AttackEditor : Editor
{
    private AttackController c;


    public void OnSceneGUI()
    {
        c = this.target as AttackController;
        Handles.color = Color.green;
        Handles.DrawWireDisc(c.transform.position, Vector3.forward, c.attackRadius);
        // radius
    }
}
