using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Spells;
using UnityEngine;


namespace Scripts.Spells
{
    [System.Serializable]
    public class SavedSpell : SpellBase
    {
        public List<SavedSpellEffectContainer> spellEffects;
    }
}