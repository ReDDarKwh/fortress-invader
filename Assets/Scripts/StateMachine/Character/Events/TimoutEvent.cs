using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;

namespace Scripts.Characters
{
    [CreateAssetMenu(fileName = "TimoutEvent", menuName = "StateMachine/Events/Enemy/TimoutEvent")]
    public class TimoutEvent : BaseEvent
    {
        public float time;

        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            message = EventMessage.EmptyMessage;
            return Time.time - activeLinking.timeStarted > time;
        }

        public override void Init(ActiveLinking activeLinking)
        {
            time = activeLinking.GetValueOrDefault<float>("time", time);
        }

    }
}