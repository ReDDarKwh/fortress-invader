

using UnityEditor;
using UnityEngine;
using System.Collections;
using Scripts.Characters;
using System.Linq;


[CustomEditor(typeof(StateMachine))]
public class StateMachineEditor : Editor
{

    private StateMachine c;


    public void OnSceneGUI()
    {

        c = this.target as StateMachine;
        if (c.debugShowStates)
        {

            c = this.target as StateMachine;
            var style = new GUIStyle();
            style.fontSize = 12;
            style.normal.textColor = Color.white;

            Handles.color = Color.red;
            Handles.Label(
                c.transform.position + new Vector3(-1, 1),
             "States: " + string.Join(", ", c.GetActiveStates().Select(x => x.stateName)), style);

            //     Handles.Label(
            //       c.transform.position + new Vector3(-1, 1),
            //    "V: " + c.GetComponent<Rigidbody2D>().velocity.magnitude, style);


        }

    }
}