using Scripts.NPC;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using MoreLinq;

namespace Scripts.Characters
{
    [CreateAssetMenu(fileName = "CharacterTargetInViewRangeEvent", menuName = "StateMachine/Events/Enemy/CharacterTargetInViewRangeEvent")]
    public class CharacterTargetInViewRangeEvent : BaseEvent
    {

        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            var npc = character.GetComponent<NonPlayerCharacter>();

            var characters = npc.getTargetsInViewRange().Select(x => x.GetComponent<Character>()).Where(x => x != null);

            var target = npc.TargetInView(characters
            .MinBy(x => (npc.transform.position - x.transform.position).magnitude)
            .FirstOrDefault()?.gameObject);
            message = new EventMessage() { target = target?.gameObject };

            return target != null;
        }

        public override void Init(ActiveLinking activeLinking)
        {

        }

    }

}