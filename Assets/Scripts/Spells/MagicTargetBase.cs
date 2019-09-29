using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.Spells;
using System;
using Scripts.AI;
using Random = UnityEngine.Random;
using Scripts.Characters;

namespace Scripts.Spells
{




    public abstract class MagicTargetBase : MonoBehaviour
    {
        public Spell spell;

        public SpellCaster caster;

        public Transform follow;

        public GameObject targetPrefab;


        public bool active = true;



        public float addedNodeTravelCost = 10;


        [System.NonSerialized]
        public HashSet<Character> selectedCharacters;


        // used by effects to know where spell came from (ex: for explosion push direction)
        [System.NonSerialized]
        public Vector3 SpellOrigin;


        protected float startTime;


        public List<ParticleSystem> effects;
        private List<SpellEffect> activeSpellEffects;
        private Dictionary<SpellEffect, float> lastEmissionTimes;

        protected List<Nav2dNode> affectedNodes;
        public abstract void Cast();

        public bool spellDone = false;



        public abstract void OnEmitEffect(SpellEffect effect);

        public abstract void Dispell();




        protected void BaseStart()
        {

            if (selectedCharacters == null)
            {
                selectedCharacters = new HashSet<Character>();
            }


            // if null magic target is for selection only (doest have effects)
            if (spell != null)
            {

                if (affectedNodes != null)
                {
                    foreach (var item in affectedNodes)
                    {
                        item.travelCost += spell.travelCost;
                    }
                }

                startTime = Time.time;
                activeSpellEffects = new List<SpellEffect>(spell.spellEffects.Select(x => x.spellEffect));
                lastEmissionTimes = new Dictionary<SpellEffect, float>();


                foreach (var effect in activeSpellEffects)
                {
                    lastEmissionTimes.Add(
                        effect,
                        effect.emitOnStart ? float.MinValue : startTime
                    );
                }

            }




        }



        private void EmitEffect(SpellEffect effect)
        {

            // stuff that affects characters

            if (effect.usesCharacterStateMachine)
            {
                foreach (var c in selectedCharacters)
                {
                    if (Random.value < effect.emissionChance)
                    {
                        var stateMachine = c.GetComponent<StateMachine>();
                        var msg = new EventMessage { pos = SpellOrigin, target = this.gameObject };


                        // start states directly 
                        foreach (var s in effect.effectEntryStates)
                        {
                            stateMachine.StartState(s, new EventStateLinking
                            {
                                eventResponse = msg

                            }, false);
                        }

                        // start states indirecly through events
                        foreach (var e in effect.effectEntryEvents)
                            stateMachine.TriggerEvent(e, msg);
                    }
                }
            }
            else
            {
                // effect that is not applied to character
                // Using the state class to contain  effect logic
                foreach (var s in effect.effectEntryStates)
                {
                    var msg = new EventMessage { pos = SpellOrigin, target = this.gameObject };
                    s.Enter(null, msg, null);
                }
            }

            OnEmitEffect(effect);
        }



        protected void UpdateActiveEffects()
        {

            // if target as no spell or spell is over. skip update
            if (spell == null || spellDone)
                return;

            // update active spell effects;

            // no active effects delete object.

            if (activeSpellEffects.Count < 1)
            {
                this.spellDone = true;
                BeforeDispell();
                Dispell();
                return;
            }

            foreach (var effect in activeSpellEffects)
            {
                if (Time.time - lastEmissionTimes[effect] >= effect.intervalTime)
                {
                    lastEmissionTimes[effect] = Time.time;

                    EmitEffect(effect);
                }
            }

            activeSpellEffects = activeSpellEffects
            .Where(
                x =>
                {
                    return Time.time - startTime < x.duration;
                }).ToList();

        }

        private void BeforeDispell()
        {
            //remove nav2d nodes cost
            if (affectedNodes != null)
                foreach (var item in affectedNodes)
                {
                    item.travelCost -= addedNodeTravelCost;
                }
        }
    }
}