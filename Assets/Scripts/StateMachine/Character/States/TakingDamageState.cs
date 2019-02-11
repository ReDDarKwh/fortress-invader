using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


[CreateAssetMenu(fileName = "TakingDamageState", menuName = "StateMachine/States/TakingDamageState")]
public class TakingDamageState : BaseState
{
    public float dmg;

    public TakingDamageState()
    {
        this.stateName = "taking_damage";
    }
    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        var c = stateMachine.GetComponent<Character>();
        c.AddDamage(dmg);
    }

    public override void Leave(StateMachine stateMachine)
    {

    }
    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}
