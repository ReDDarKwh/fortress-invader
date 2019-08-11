using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DestroyLogic", menuName = "Spells/EffectLogic/DestroyLogic")]
public class DestroyLogic : BaseState
{

    // stateMachine and activeLinking always null
    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        Destroy(eventResponse.target);
    }

    // unused for state
    public override void Leave(StateMachine stateMachine) { }
    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking) { }

}