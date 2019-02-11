using Scripts.NPC;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SuspiciousState", menuName = "StateMachine/States/Enemy/SuspiciousState")]
public class SuspiciousState : BaseState
{

    public float maxSuspiciousTime = 10;

    [Range(0, 1)]
    public float viewWeight = 0.2f;

    public SuspiciousState()
    {
        this.stateName = "suspicious";
    }
    override public void Leave(StateMachine stateMachine)
    {

    }

    private float getSuspiciousTime(NonPlayerCharacter npc, Character target)
    {

        var normDistance = (npc.transform.position - target.transform.position).magnitude / npc.VisionRadius;

        var distanceWeight = 1 - viewWeight;

        var suspiciousTime =
        (1 - target.visibility) * maxSuspiciousTime * viewWeight +
        normDistance * maxSuspiciousTime * distanceWeight;


        return suspiciousTime;
    }

    override public void Enter(StateMachine stateMachine, EventMessage eventMessage, ActiveLinking activeLinking)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

        npc.VisionAngleModifier = 1.8f;
        npc.VisionRadiusModifier = 1.1f;

        npc.characterTarget = eventMessage.target.GetComponent<Character>();

        activeLinking.linkingProperties.Add("time", getSuspiciousTime(npc, npc.characterTarget));
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {
    }
}



