using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SeachingState", menuName = "StateMachine/States/Enemy/SeachingState")]
public class SeachingState : BaseState
{
    public SeachingState()
    {
        this.stateName = "searching";
    }
    override public void Leave(StateMachine stateMachine)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();
        npc.nav2DAgent.endMovement();
    }
    override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

        npc.character.currentSpeed = CharacterSpeed.NORMAL;

        npc.VisionAngleModifier = 1.8f;
        npc.VisionRadiusModifier = 1.1f;

        npc.MoveToOrFollow(npc.searchingPosition);
        activeLinking.linkingProperties.Add("target_position", npc.searchingPosition);
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}



