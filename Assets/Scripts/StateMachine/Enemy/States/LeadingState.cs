using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LeadingState", menuName = "StateMachine/States/Enemy/LeadingState")]
public class LeadingState : BaseState
{
    public LeadingState()
    {
        this.stateName = "leading";
    }
    override public void Leave(StateMachine stateMachine)
    {
        var follower = stateMachine.GetComponent<Leader>();
        follower.enabled = false;
    }
    override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        var follower = stateMachine.GetComponent<Leader>();
        follower.enabled = true;
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}



