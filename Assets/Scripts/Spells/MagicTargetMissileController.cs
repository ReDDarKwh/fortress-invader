using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Scripts.Spells
{
    public class MagicTargetMissileController : MagicTargetBase
    {

        //public ParticleSystem particlesSystemParent;

        [System.NonSerialized]
        public int maxTargets;


        public float missileInitialForce;


        public GameObject missilePrefab;

        void Start()
        {
            BaseStart();

            // if (spell = null)
            // {
            //     selectedCharacters = new HashSet<Character>();
            // }
        }



        // Update is called once per frame
        void Update()
        {
            transform.position = follow.position;

            UpdateActiveEffects();
        }



        void OnDestroy()
        {
            if (spell == null)
                foreach (var character in selectedCharacters)
                {
                    character.IsSelected.Value = false;
                }
        }


        // used when controller is for selection only
        public override void Cast()
        {

            foreach (var characterTarget in selectedCharacters)
            {


                // create the missile
                var missile = Instantiate(missilePrefab, caster.transform.position, Quaternion.identity);

                var missileController = missile.GetComponent<MagicMissileController>();

                missileController.characterTarget = characterTarget;
                missileController.selectionMissileTargetController = this;


                var angle = Random.Range(0, Mathf.PI * 2);
                missileController.velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * missileInitialForce;


                // create and init spell area



                // temporary. Mana will be spent gradually

                //caster.currentMana.Value -= caster.selectedSpell.Value.manaCost;
            }

            foreach (var c in selectedCharacters)
            {
                c.IsSelected.Value = false;
            }
            selectedCharacters.Clear();


        }

        public void Select()
        {

            var collider = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("AI"));

            if (collider != null)
            {
                var character = collider.GetComponent<Character>();

                if (selectedCharacters.Contains(character))
                {
                    selectedCharacters.Remove(character);
                    character.IsSelected.Value = false;
                }
                else
                {
                    if (selectedCharacters.Count <
                     caster.selectedSpell.Value.maxTargets)
                    {
                        selectedCharacters.Add(character);
                        character.IsSelected.Value = true;
                    }
                }

            }
        }


        void DebugDrawBox(Vector2 point, Vector2 size, float angle, Color color, float duration)
        {

            var orientation = Quaternion.Euler(0, 0, angle);

            // Basis vectors, half the size in each direction from the center.
            Vector2 right = orientation * Vector2.right * size.x / 2f;
            Vector2 up = orientation * Vector2.up * size.y / 2f;

            // Four box corners.
            var topLeft = point + up - right;
            var topRight = point + up + right;
            var bottomRight = point - up + right;
            var bottomLeft = point - up - right;

            // Now we've reduced the problem to drawing lines.
            Debug.DrawLine(topLeft, topRight, color, duration);
            Debug.DrawLine(topRight, bottomRight, color, duration);
            Debug.DrawLine(bottomRight, bottomLeft, color, duration);
            Debug.DrawLine(bottomLeft, topLeft, color, duration);
        }

        public void SelectRect(Vector2 point, Vector2 size)
        {

            if (size.magnitude < 1)
            {
                return;
            }

            DebugDrawBox(point, size, 0, Color.red, 0);

            var colliders = Physics2D.OverlapBoxAll(point, size, 0, LayerMask.GetMask("AI"));

            foreach (var collider in colliders)
            {
                var character = collider.GetComponent<Character>();

                if (selectedCharacters.Count <
                 caster.selectedSpell.Value.maxTargets)
                {
                    selectedCharacters.Add(character);
                    character.IsSelected.Value = true;
                }

            }
        }

        public override void Dispell()
        {
            Destroy(gameObject, 5);

            active = false;
        }

        public override void OnEmitEffect(SpellEffect effect)
        {

        }


    }

}
