

using UnityEditor;
using UnityEngine;
using System.Collections;
using Scripts.Characters;
using Scripts.NPC;

[CustomEditor(typeof(NonPlayerCharacter))]
public class EnemyEditor : Editor
{

    private NonPlayerCharacter c;


    public void OnSceneGUI()
    {
        c = this.target as NonPlayerCharacter;
        Handles.color = Color.red;

        Handles.DrawWireDisc(c.transform.position, Vector3.forward, c.earshotRadius);

        Handles.DrawWireArc(c.transform.position, c.transform.forward,
          (Quaternion.Euler(0, 0, -c.VisionAngle / 2) * c.transform.right) * c.VisionRadius,
          c.VisionAngle, c.VisionRadius);

        Handles.DrawLine(c.transform.position, c.transform.position + (Quaternion.Euler(0, 0, -c.VisionAngle / 2) * c.transform.right) * c.VisionRadius);
        Handles.DrawLine(c.transform.position, c.transform.position + (Quaternion.Euler(0, 0, c.VisionAngle / 2) * c.transform.right) * c.VisionRadius);

        // radius
    }
}