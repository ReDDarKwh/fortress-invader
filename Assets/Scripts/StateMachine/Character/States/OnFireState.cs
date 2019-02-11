using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


[CreateAssetMenu(fileName = "OnFireState", menuName = "StateMachine/States/OnFireState")]
public class OnFireState : BaseState
{
    public float duration;
    public float fireDamageInterval;
    public float fireDmg;

    public OnFireState()
    {
        this.stateName = "on_fire";
    }
    public override void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        //set time before stunnedState leaves
        activeLinking.linkingProperties.Add("time", duration);

        activeLinking.linkingProperties.Add("lastFireDamageTime", activeLinking.timeStarted);

        var c = stateMachine.GetComponent<Character>();


        c.effectIndicators.fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = c.effectIndicators.fireEffect.main;
        main.duration = duration;
        c.effectIndicators.fireEffect.Play();


    }

    public override void Leave(StateMachine stateMachine)
    {
        var c = stateMachine.GetComponent<Character>();
        c.effectIndicators.fireEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    public override void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

        var lastFireDamageTime = activeLinking.GetValueOrDefault<float>("lastFireDamageTime");

        if (Time.time - lastFireDamageTime > fireDamageInterval)
        {
            var c = stateMachine.GetComponent<Character>();
            c.AddDamage(fireDmg);

            activeLinking.linkingProperties["lastFireDamageTime"] = Time.time;
        }
    }
}
