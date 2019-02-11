using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Scripts.Spells
{

    [RequireComponent(typeof(Character))]
    public class SpellCaster : MonoBehaviour
    {
        public FloatReactiveProperty maxMana;
        public ReactiveProperty<float> currentMana = new ReactiveProperty<float>();

        public ReactiveProperty<Spell> selectedSpell = new ReactiveProperty<Spell>();

        public List<Spell> spells;

        // Use this for initialization
        void Start()
        {


            currentMana.Value = maxMana.Value;
        }

        // Update is called once per frame
        void Update()
        {

        }

    }

}

