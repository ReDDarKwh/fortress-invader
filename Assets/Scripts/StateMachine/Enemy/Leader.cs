﻿using System.Collections;
using System.Collections.Generic;
using Scripts.AI;
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

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<Nav2DAgent>();
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!navAgent.isMoving && !navAgent.isWaitingForPath)
        {
            navAgent.setTargetWithPath(path, pathFindingTargetRadius, pathMoveSpeed, true);
        }
    }
}
