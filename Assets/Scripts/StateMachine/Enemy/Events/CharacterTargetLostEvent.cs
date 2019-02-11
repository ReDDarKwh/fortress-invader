using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;

namespace Scripts.Characters
{
    [CreateAssetMenu(fileName = "CharacterTargetLostEvent", menuName = "StateMachine/Events/Enemy/CharacterTargetLostEvent")]
    public class CharacterTargetLostEvent : BaseEvent
    {
        public override bool Check(GameObject character, ActiveLinking activeLinking, out EventMessage message)
        {
            var characterTarget = character.GetComponent<NonPlayerCharacter>()?.characterTarget;


            var targetInView = character.GetComponent<NonPlayerCharacter>()
            .TargetInView(characterTarget?.gameObject);

            message = new EventMessage() { target = characterTarget?.gameObject };

            // if suspected target lost return that target.
            return targetInView == null;
        }

        public override void Init(ActiveLinking activeLinking)
        {

        }

    }

}