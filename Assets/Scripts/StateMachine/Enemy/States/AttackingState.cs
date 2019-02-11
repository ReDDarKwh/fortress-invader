using UnityEngine;
using System.Collections.Generic;


namespace Scripts.NPC
{
    [CreateAssetMenu(fileName = "AttackingState", menuName = "StateMachine/States/Enemy/AttackingState")]
    public class AttackingState : BaseState
    {
        public AttackingState()
        {
            this.stateName = "attacking";
        }
        override public void Leave(StateMachine stateMachine)
        {

        }
        override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {
            NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

            activeLinking.linkingProperties.Add("target_position", npc.characterTarget.transform);
        }
        override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {

            NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();
            var npcToTarget = npc.characterTarget.transform.position - npc.transform.position;

            npc.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(npcToTarget.y, npcToTarget.x) * Mathf.Rad2Deg);

        }
    }
}



