


using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

namespace Scripts.NPC
{

    [CreateAssetMenu(fileName = "UncertainState", menuName = "StateMachine/States/Enemy/UncertainState")]
    public class UncertainState : BaseState
    {
        public UncertainState()
        {
            this.stateName = "uncertain";
        }
        override public void Leave(StateMachine stateMachine)
        {
        }
        override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {

            NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

            npc.nav2DAgent.addSeparationIgnored(eventResponse.target);


            //clear any character target;
            npc.characterTarget = null;


            npc.searchingPosition = eventResponse.target != null ?
                eventResponse.target.transform.position : (
                    npc.characterTarget == null ?
                    npc.transform.position :
                    npc.characterTarget.transform.position
                );
        }
        override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {
        }
    }

}


