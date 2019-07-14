using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.AI;

namespace Scripts.Spells
{
    public class MagicTargetCircleController : MagicTargetBase
    {

        public float radius;
        public float safeDistance;

        //public ParticleSystem particlesSystemParent;

        void Start()
        {
            Nav2D nav2D = GameObject.FindWithTag("MainNavGrid").GetComponent<Nav2D>();
            if (spell != null)
            {
                if (spell.travelCost > 0)
                {
                    // apply travel cost to nav2d nodes under circle target
                    affectedNodes = nav2D.GetNodesInCircle(transform.position, radius + safeDistance);
                    //
                }
            }

            BaseStart();
            setRadius(radius);
        }

        void UpdateRadius()
        {


        }

        // Update is called once per frame
        void Update()
        {
            transform.position = follow.position;

            // update spell selected characters

            var characters =
              Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("AI"))
              .Select(x => x.GetComponent<Character>()).Where(x => x != null);

            foreach (var character in selectedCharacters.Where(x => !characters.Contains(x)))
            {
                character.IsSelected.Value = false;
            }

            selectedCharacters.Clear();

            foreach (var character in characters)
            {
                selectedCharacters.Add(character);
                character.IsSelected.Value = true;
            }



            UpdateActiveEffects();
        }

        public void setRadius(float radius)
        {
            this.radius = radius;

            // update inner effects radius
            foreach (var effect in effects)
            {
                foreach (var child in effect.GetComponentsInChildren<ParticleSystem>().Where(
                    x => x.tag == "EffectAjustable"
                    ))
                {
                    var shape = child.shape;
                    shape.radius = radius;
                }
            }


            // update collider radius

            this.GetComponent<CircleCollider2D>().radius = radius;
        }

        void OnDestroy()
        {
            foreach (var character in selectedCharacters)
            {
                character.IsSelected.Value = false;
            }
        }


        // used when controller is for selection only
        public override void Cast()
        {


            // create and init spell area
            var spellTarget = Instantiate(targetPrefab, transform.position, Quaternion.identity);

            var circleTarget = spellTarget.GetComponent<MagicTargetCircleController>();

            circleTarget.caster = caster;
            circleTarget.spell = caster.selectedSpell.Value;
            circleTarget.SpellOrigin = spellTarget.transform.position;

            circleTarget.effects = circleTarget.spell.spellEffects.Where(x => x.circleEffect != null).Select(effect =>
                    {

                        // init each particleSystems of each spell effect.

                        var particleSystem = Instantiate(effect.circleEffect, circleTarget.transform.position, Quaternion.identity)
                          .GetComponent<ParticleSystem>();

                        particleSystem.transform.SetParent(circleTarget.transform);

                        var main = particleSystem.main;

                        main.startLifetime = main.duration = Mathf.Max(1, effect.duration);

                        particleSystem.Play();

                        return particleSystem;
                    }).ToList();

            circleTarget.follow = circleTarget.transform;
            circleTarget.radius = this.radius;

            // temporary. Mana will be spent gradually

            //caster.currentMana.Value -= caster.selectedSpell.Value.manaCost;
        }

        public override void Dispell()
        {
            Destroy(gameObject, 5);

            active = false;
        }

        public override void OnEmitEffect(SpellEffect effect)
        {
            var noiseEmitter = GetComponent<NoiseEmitter>();

            if (effect.noiseRadius > 0)
            {

                noiseEmitter.EmitNoise(radius + effect.noiseRadius);
            }
        }


    }

}
