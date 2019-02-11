

using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Scripts.Spells
{


    [CreateAssetMenu(fileName = "SpellEffect", menuName = "Spells/SpellEffect")]
    public class SpellEffect : ScriptableObject
    {
        public string effectName;
        public GameObject circleEffect;

        [Tooltip("Total time the effect can be emitted")]
        public float duration;


        [Tooltip("time (in sec) between each effect emission. Zero means instant")]
        public float intervalTime;

        public float emissionChance;
        //public SpellEffectType emissionType;
        public bool emitOnStart;
        public List<BaseEvent> effectEntryEvents;
        public float noiseRadius;

    }

    // public enum SpellEffectType
    // {
    //     INSTANT,
    //     TIMED
    // }

}


