using UnityEngine;


namespace Scripts.Spells
{
    public abstract class SpellBase
    {
        public string spellName;
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

