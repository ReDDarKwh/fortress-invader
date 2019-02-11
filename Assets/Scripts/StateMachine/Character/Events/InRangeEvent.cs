using System.Collections;
using System.Collections.Generic;
using Scripts.Characters;
using UnityEngine;

namespace Scripts.NPC
{
    [CreateAssetMenu(fileName = "InRangeEvent", menuName = "StateMachine/Events/Enemy/InRangeEvent")]
    public class InRangeEvent : BaseEvent
    {
        public float range;
        public bool movingTarget = false;

        [System.NonSerialized]
        public Vector3 targetPosition;

        [System.NonSerialized]
        public Transform tranform;

        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            message = EventMessage.EmptyMessage;
            if (movingTarget)
            {
                return tranform == null ? false : (character.transform.position - tranform.position).magnitude < range;
            }
            return (character.transform.position - targetPosition).magnitude < range;
        }

        public override void Init(ActiveLinking activeLinking)
        {
            range = activeLinking.GetValueOrDefault<float>("range", range);

            if (movingTarget)
            {
                tranform = activeLinking.GetValueOrDefault<Transform>("target_position", tranform);
            }
            else
            {
                targetPosition = activeLinking.GetValueOrDefault<Vector3>("target_position", targetPosition);
            }
        }
    }
}