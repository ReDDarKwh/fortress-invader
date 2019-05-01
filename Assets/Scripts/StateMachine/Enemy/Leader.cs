using System.Collections;
using System.Collections.Generic;
using Scripts.AI;
using TownGenerator.Geom;
using UnityEngine;

public class Leader : MonoBehaviour
{

    private Nav2DAgent navAgent;

    [System.NonSerialized]
    public Character character;

    [System.NonSerialized]
    public List<Vector3> path;

    public float pathFindingTargetRadius;
    public float pathMoveSpeed;
    public float towerDetectRadius;
    public string[] towerLayers;




    private bool disableTowerDetect;
    [System.NonSerialized]
    internal List<Vector3> currentPath;
    internal int startPathIndex;


    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<Nav2DAgent>();
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {

        var closeToTower = Physics2D.OverlapCircle(transform.position, towerDetectRadius, LayerMask.GetMask(towerLayers));

        if (!closeToTower)
        {
            disableTowerDetect = false;
        }

        if (closeToTower && !disableTowerDetect)
        {
            disableTowerDetect = true;
            navAgent.direction = navAgent.direction * -1;
        }

        if ((!navAgent.isMoving && !navAgent.isWaitingForPath))
        {
            navAgent.setTargetWithPath(currentPath, pathFindingTargetRadius, pathMoveSpeed, true, startPathIndex);
        }
    }
}
