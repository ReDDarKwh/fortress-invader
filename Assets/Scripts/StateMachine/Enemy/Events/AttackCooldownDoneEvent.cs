using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "AttackCooldownDoneEvent", menuName = "StateMachine/Events/Enemy/AttackCooldownDoneEvent")]
public class AttackCooldownDoneEvent : BaseEvent
{
    public float coolDownTime;
    private float lastTrigger;

    public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
    {

        message = EventMessage.EmptyMessage;

        if (Time.time - lastTrigger > coolDownTime)
        {
            lastTrigger = Time.time;

            return true;
        }

        return false;
    }

    public override void Init(ActiveLinking activeLinking)
    {

    }
}
