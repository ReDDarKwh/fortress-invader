using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Spells
{
    public class SpellEffectContainer
    {
        public SpellEffect spellEffect;
        public Vector3 position;
    }


    [System.Serializable]
    public class SavedSpellEffectContainer
    {
        public string item1;
        public Vector3 item2;
        public SavedSpellEffectContainer(string effectName, Vector3 position)
        {
            this.item1 = effectName;
            this.item2 = position;
        }
    }
}
