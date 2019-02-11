using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Scripts.AI;

[CreateAssetMenu(fileName = "StunnedState", menuName = "StateMachine/States/StunnedState")]
public class StunnedState : BaseState
{
    public float duration;

    public StunnedState()
    {
        this.stateName = "stunned";
    }
    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        //set time before stunnedState leaves
        activeLinking.linkingProperties.Add("time", duration);

        var c = stateMachine.GetComponent<Character>();


        c.effectIndicators.stunnedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = c.effectIndicators.stunnedEffect.main;
        main.duration = duration;
        c.effectIndicators.stunnedEffect.Play();


        // disable agent2d for pushing around

        var agent = stateMachine.GetComponent<Nav2DAgent>();
        agent.enabled = false;

        Debug.Log("disabled agent");

    }

    public override void Leave(StateMachine stateMachine)
    {
        var c = stateMachine.GetComponent<Character>();
        c.effectIndicators.stunnedEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);


        var agent = stateMachine.GetComponent<Nav2DAgent>();
        agent.enabled = true;


        Debug.Log("enabled agent");

    }
    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

    }
}
