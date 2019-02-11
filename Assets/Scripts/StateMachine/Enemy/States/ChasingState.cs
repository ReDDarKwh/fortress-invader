using UnityEngine;
using System.Collections.Generic;


namespace Scripts.NPC
{
    [CreateAssetMenu(fileName = "ChasingState", menuName = "StateMachine/States/Enemy/ChasingState")]
    public class ChasingState : BaseState
    {
        public ChasingState()
        {
            this.stateName = "chasing";
        }
        override public void Leave(StateMachine stateMachine)
        {
            NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();
            npc.nav2DAgent.endMovement();
        }
        override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {

            NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();
            if (npc.characterTarget == null)
                return;


            npc.VisionAngleModifier = 2.5f;
            npc.VisionRadiusModifier = 1.3f;


            npc.nav2DAgent.removeSeparationIgnored(npc.characterTarget.gameObject);

            activeLinking.linkingProperties.Add("target_position", npc.characterTarget.transform);
            activeLinking.linkingProperties.Add("force_pathfinding_update", true);
        }
        override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {

            NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();


            npc.character.currentSpeed = CharacterSpeed.FAST;

            if (npc.characterTarget != null && npc.characterTarget.transform != null)
            {
                if (activeLinking.GetValueOrDefault<bool>("force_pathfinding_update") ||
                 (npc.characterTarget.transform.position - npc.oldPositionTarget).magnitude > npc.pathToTargetUpdateDistance)
                {
                    npc.MoveToOrFollow(npc.characterTarget.transform.position);
                    npc.oldPositionTarget = npc.characterTarget.transform.position;

                    activeLinking.linkingProperties["force_pathfinding_update"] = false;
                }
            }

        }
    }
}



