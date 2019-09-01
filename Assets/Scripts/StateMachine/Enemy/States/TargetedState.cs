

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TargetedState", menuName = "StateMachine/States/TargetedState")]
public class TargetedState : BaseState
{

    public TargetedState()
    {
        this.stateName = "TargetedState";
    }

    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {

    }

    public override void Leave(StateMachine stateMachine)
    {

    }

    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }

}
