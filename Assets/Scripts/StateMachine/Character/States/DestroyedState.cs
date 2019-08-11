

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DestroyedState", menuName = "StateMachine/States/DestroyedState")]
public class DestroyedState : BaseState
{

    public DestroyedState()
    {
        this.stateName = "DestroyedState";
    }

    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        Destroy(stateMachine.gameObject);
    }

    public override void Leave(StateMachine stateMachine)
    {

    }

    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }

}
