
using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;

namespace Scripts.NPC
{
    [CreateAssetMenu(fileName = "OnEndOfNavPathEvent", menuName = "StateMachine/Events/Enemy/OnEndOfNavPathEvent")]
    public class OnEndOfNavPathEvent : BaseEvent
    {

        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            var npc = character.GetComponent<NonPlayerCharacter>();
            message = EventMessage.EmptyMessage;

            return !npc.nav2DAgent.isMoving();
        }

        public override void Init(ActiveLinking activeLinking)
        {
        }
    }
}