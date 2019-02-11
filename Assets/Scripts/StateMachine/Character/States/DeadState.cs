using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AI;


[CreateAssetMenu(fileName = "DeadState", menuName = "StateMachine/States/DeadState")]
public class DeadState : BaseState
{

    public DeadState()
    {
        this.stateName = "dead";
    }

    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {

        stateMachine.GetComponent<Animator>()?.SetBool("dead", true);
        //temporary
        //Destroy(stateMachine.gameObject);


        // disable agent2d for pushing around

        var agent = stateMachine.GetComponent<Nav2DAgent>();
        agent.enabled = false;
    }

    public override void Leave(StateMachine stateMachine)
    {

    }

    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }


}
