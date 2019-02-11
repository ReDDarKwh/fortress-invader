using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.Spells;
using System;
using Scripts.AI;

namespace Scripts.Spells
{
    public abstract class MagicTargetBase : MonoBehaviour
    {
        public Spell spell;
        public Nav2D nav2D;

        public float radius;

        public float addedNodeTravelCost = 10;


        protected List<Character> selectedCharacters;
        protected float startTime;


        private List<SpellEffect> activeSpellEffects;
        private Dictionary<SpellEffect, float> lastEmissionTimes;

        private List<Nav2dNode> affectedNodes;


        public abstract void Cast();


        public abstract void OnEmitEffect(SpellEffect effect);

        public abstract void Dispell();

        private void EmitEffect(SpellEffect effect)
        {
            foreach (var c in selectedCharacters)
            {
                var stateMachine = c.GetComponent<StateMachine>();
                var msg = new EventMessage { target = this.gameObject };

                foreach (var e in effect.effectEntryEvents)
                    stateMachine.TriggerEvent(e, msg);
            }

            OnEmitEffect(effect);
        }

        protected void BaseStart()
        {


            // if null magic target is for selection only (doest have effects)
            if (spell == null)
                return;

            nav2D = GameObject.FindWithTag("MainNavGrid").GetComponent<Nav2D>();

            startTime = Time.time;
            activeSpellEffects = new List<SpellEffect>(spell.spellEffects);
            lastEmissionTimes = new Dictionary<SpellEffect, float>();

            foreach (var effect in activeSpellEffects)
            {
                lastEmissionTimes.Add(
                    effect,
                    effect.emitOnStart ? float.MinValue : startTime
                );
            }


            // apply travel cost to nav2d nodes under circle target

            affectedNodes = nav2D.GetNodesInCircle(transform.position, radius);

            foreach(var item in affectedNodes) {

                item.travelCost += addedNodeTravelCost;
            }

            //



        }


        protected void UpdateActiveEffects()
        {

            if (spell == null)
                return;

            // update active spell effects;

            // no active effects delete object.

            if (activeSpellEffects.Count < 1)
            {
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
            foreach(var item in affectedNodes) {

                item.travelCost -= addedNodeTravelCost;            
            
            }
        }
    }
}