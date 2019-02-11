using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EmptyState", menuName = "StateMachine/States/EmptyState")]
public class EmptyState : BaseState
{

    public EmptyState()
    {
        this.stateName = "empty";
    }

    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {

        //temporary
        //Destroy(stateMachine.gameObject);
    }

    public override void Leave(StateMachine stateMachine)
    {

    }

    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }

}
