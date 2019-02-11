using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;

namespace Scripts.Characters
{
    [CreateAssetMenu(fileName = "AlwaysFalseEvent", menuName = "StateMachine/Events/AlwaysFalseEvent")]
    public class AlwaysFalseEvent : BaseEvent
    {
        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            message = EventMessage.EmptyMessage;
            return false;
        }

        public override void Init(ActiveLinking activeLinking)
        {
        }
    }
}


