using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;
using Scripts.Spells;

[CreateAssetMenu(fileName = "CircleTargetScaredState", menuName = "StateMachine/States/Enemy/CircleTargetScaredState")]
public class CircleTargetScaredState : BaseState
{

    public float runDistanceFromOuterRadius = 1;

    public CircleTargetScaredState()
    {
        this.stateName = "circle_target_scared";
    }
    override public void Leave(StateMachine stateMachine)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();
        npc.nav2DAgent.endMovement();
    }
    override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

        MagicTargetCircleController target = eventResponse.target.GetComponent<MagicTargetCircleController>();

        npc.character.currentSpeed = CharacterSpeed.FAST;

        var moveVec = target.transform.position + ((npc.transform.position - target.transform.position).normalized * (target.radius + runDistanceFromOuterRadius));


        Debug.DrawLine(target.transform.position, moveVec, Color.green, 10);

        npc.MoveToOrFollow(moveVec);

        activeLinking.linkingProperties.Add("target_position", moveVec);
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}



