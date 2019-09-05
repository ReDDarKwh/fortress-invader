




using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Scripts.Spells
{

    [CreateAssetMenu(fileName = "Spell", menuName = "Spells/Spell")]
    public class Spell : ScriptableObject
    {
        public string spellName;
        public List<SpellEffect> spellEffects;
        public SpellTarget spellTarget;
        public float createCost;
        public float manaCost;
        [Header("Cost for AI to travel in a area with this spell active")]
        public float travelCost;

        [Header("Missile target settings")]
        public int maxTargets;

        [Header("Circle/Aura target settings")]
        public float maxRadius;

    }
}

