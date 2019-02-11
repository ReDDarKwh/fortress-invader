

using System.Collections;
using UniRx;
using UnityEngine;


namespace Scripts.Spells
{
    public enum SpellTarget
    {
        MISSILE,
        SELF, // relative to player
        AURA, // relative to player
        CONE,
        CIRCLE,
        SQUARE,
        LASER, // relative to player
        PROJECTILE // relative to player
    }
}

