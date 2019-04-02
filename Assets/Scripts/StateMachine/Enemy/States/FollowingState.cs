using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FollowingState", menuName = "StateMachine/States/Enemy/FollowingState")]
public class FollowingState : BaseState
{
    public FollowingState()
    {
        this.stateName = "following";
    }
    override public void Leave(StateMachine stateMachine)
    {
        var follower = stateMachine.GetComponent<Follower>();
        follower.enabled = false;
    }
    override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        var follower = stateMachine.GetComponent<Follower>();
        follower.enabled = true;
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}



