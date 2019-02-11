

using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;
using Scripts.AI;

namespace Scripts.NPC
{
    [CreateAssetMenu(fileName = "SlashingState", menuName = "StateMachine/States/Enemy/SlashingState")]
    public class SlashingState : BaseState
    {
        public SlashingState()
        {
            this.stateName = "slashing";
        }
        override public void Leave(StateMachine stateMachine)
        {
            var npc = stateMachine.GetComponent<NonPlayerCharacter>();
            npc.nav2DAgent.NeightborAvoidance = true;


        }
        override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {

            var npc = stateMachine.GetComponent<NonPlayerCharacter>();
            var character = stateMachine.GetComponent<Character>();

            character.currentSpeed = CharacterSpeed.FAST;

            npc.nav2DAgent.setTarget(
                npc.characterTarget.transform.position -
                (npc.characterTarget.transform.position - character.transform.position).normalized * 2,
                NavAgentMode.DIRECT,
                1,
                character.GetSpeed(),
                false
            );
        }
        override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {
        }
    }
}



