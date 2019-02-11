using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;

namespace Scripts.Characters
{
    [CreateAssetMenu(fileName = "NoHealthEvent", menuName = "StateMachine/Events/NoHealthEvent")]
    public class NoHealthEvent : BaseEvent
    {
        public override bool Check(GameObject gameObject, ActiveLinking activeLinking, out EventMessage message)
        {
            var character = gameObject.GetComponent<Character>();

            message = EventMessage.EmptyMessage;

            return character.currentHealth.Value <= 0;
        }

        public override void Init(ActiveLinking activeLinking)
        {
        }
    }
}


