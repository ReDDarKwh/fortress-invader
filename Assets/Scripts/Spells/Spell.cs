




using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Scripts.Spells
{
    public class Spell
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

