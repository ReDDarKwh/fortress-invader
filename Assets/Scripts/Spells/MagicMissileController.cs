using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Scripts.Spells
{
    public class MagicMissileController : MonoBehaviour
    {
        // Start is called before the first frame update

        [System.NonSerialized]
        public Character characterTarget;



        [System.NonSerialized]
        public MagicTargetMissileController selectionMissileTargetController;

        public float collisionRadius;

        [System.NonSerialized]
        public Vector3 velocity;

        public float mass;
        public float maxSteeringForce;
        public float maxSteeringSpeed;

        public float speed;

        public GameObject flamesParticleSystem;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            var missileToTargetVec = (characterTarget.transform.position - transform.position);



            if (missileToTargetVec.magnitude < collisionRadius)
            {

                // cast the spell

                var spellTarget = Instantiate(selectionMissileTargetController.targetPrefab, characterTarget.transform.position, Quaternion.identity);


                var missileTarget = spellTarget.GetComponent<MagicTargetMissileController>();

                missileTarget.caster = selectionMissileTargetController.caster;
                missileTarget.spell = selectionMissileTargetController.caster.selectedSpell.Value;

                missileTarget.SpellOrigin = transform.position;


                missileTarget.effects = missileTarget.spell.spellEffects.Where(x => x.missileEffect != null).Select(effect =>
                        {

                            // init each particleSystems of each spell effect.

                            var particleSystem = Instantiate(effect.missileEffect, missileTarget.transform.position, Quaternion.identity)
                              .GetComponent<ParticleSystem>();

                            particleSystem.transform.SetParent(missileTarget.transform);

                            var main = particleSystem.main;

                            main.startLifetime = main.duration = Mathf.Max(1, effect.duration);

                            particleSystem.Play();

                            return particleSystem;
                        }).ToList();

                missileTarget.follow = characterTarget.transform;
                missileTarget.selectedCharacters = new HashSet<Character>();

                missileTarget.selectedCharacters.Add(characterTarget);


                // detach the flames to be deleted later
                flamesParticleSystem.transform.parent = null;

                Destroy(flamesParticleSystem, 5);

                // delete missile.
                Destroy(this.gameObject);
            }
            else
            {

                var desiredVelocity = (missileToTargetVec.normalized) * Time.deltaTime * speed;

                var sterring = Vector3.ClampMagnitude((desiredVelocity - velocity), maxSteeringForce) / mass;

                velocity = Vector2.ClampMagnitude(velocity + sterring, maxSteeringSpeed);

                transform.position += velocity;



                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);


            }
        }



    }
}
