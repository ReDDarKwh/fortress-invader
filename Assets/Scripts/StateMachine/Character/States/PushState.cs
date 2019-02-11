using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PushState", menuName = "StateMachine/States/PushState")]
public class PushState : BaseState
{
    public float force;

    public PushState()
    {
        this.stateName = "pushed";
    }

    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        activeLinking.linkingProperties.Add("push_position", eventResponse.target);
    }
    public override void Leave(StateMachine stateMachine)
    {

    }
    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {
        var rb = stateMachine.GetComponent<Rigidbody2D>();
        rb.AddForce((
            activeLinking.GetValueOrDefault<GameObject>("push_position").transform.position - rb.transform.position
            ).normalized * force, ForceMode2D.Impulse
        );
        Debug.Log("Added force");
    }
}
