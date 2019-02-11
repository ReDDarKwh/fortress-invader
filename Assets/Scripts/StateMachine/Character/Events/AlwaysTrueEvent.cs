using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;

namespace Scripts.Characters
{
    [CreateAssetMenu(fileName = "AlwaysTrueEvent", menuName = "StateMachine/Events/AlwaysTrueEvent")]
    public class AlwaysTrueEvent : BaseEvent
    {
        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            message = EventMessage.EmptyMessage;
            return true;
        }

        public override void Init(ActiveLinking activeLinking)
        {
        }
    }
}


