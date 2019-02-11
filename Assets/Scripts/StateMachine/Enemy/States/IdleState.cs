using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "IdleState", menuName = "StateMachine/States/Enemy/IdleState")]
public class IdleState : BaseState
{
    public IdleState()
    {
        this.stateName = "idle";
    }
    override public void Leave(StateMachine stateMachine)
    {

    }
    override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

        npc.VisionAngleModifier = 1f;
        npc.VisionRadiusModifier = 1f;
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}



