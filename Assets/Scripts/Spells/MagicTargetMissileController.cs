using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Scripts.Spells
{
    public class MagicTargetMissileController : MagicTargetBase
    {

        //public ParticleSystem particlesSystemParent;

        void Start()
        {
            BaseStart();
        }



        // Update is called once per frame
        void Update()
        {
            // transform.position = follow.position;

            // // update spell selected characters

            // var characters =
            //   Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("AI"))
            //   .Select(x => x.GetComponent<Character>()).Where(x => x != null);

            // foreach (var character in selectedCharacters.Where(x => !characters.Contains(x)))
            // {
            //     character.IsSelected.Value = false;
            // }

            // selectedCharacters.Clear();

            // foreach (var character in characters)
            // {
            //     selectedCharacters.Add(character);
            //     character.IsSelected.Value = true;
            // }



            // UpdateActiveEffects();
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






        }

        public override void Dispell()
        {

        }

        public override void OnEmitEffect(SpellEffect effect)
        {

        }
    }

}
