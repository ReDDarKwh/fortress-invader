using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InvestigatingState", menuName = "StateMachine/States/Enemy/InvestigatingState")]
public class InvestigatingState : BaseState
{

    public float lookAroundAngle = 20;
    public float rotationSpeed = 1;


    public InvestigatingState()
    {
        this.stateName = "investigating";
    }
    override public void Leave(StateMachine stateMachine)
    {

    }
    override public void Enter(StateMachine stateMachine, EventMessage eventMessage, ActiveLinking activeLinking)
    {
        activeLinking.linkingProperties.Add("start_rotation", stateMachine.GetComponent<Character>().lookDirection);
    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

        var c = stateMachine.GetComponent<Character>();

        c.lookDirection = Quaternion.Euler(
            activeLinking.GetValueOrDefault<Quaternion>("start_rotation").eulerAngles +
            Vector3.forward * Mathf.Lerp(-lookAroundAngle, lookAroundAngle,
            0.5f + (Mathf.Sin((Time.time - activeLinking.timeStarted) * rotationSpeed) / 2))
        );

    }
}



