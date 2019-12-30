using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
       

        public GameObject missileTargetPrefab;
        public GameObject circleTargetPrefab;
        public GameObject circleTargetEffectPrefab;
        public Transform targetTransform;

        public Animator bodyAnimator;

        [System.NonSerialized]
        public float spellRadius;



        public GameObject spellTarget { private set; get; }


        public void Cast(bool specialCast)
        {
            if (spellTarget != null)
            {

                var spell = selectedSpell.Value;


                switch (spell.spellTarget)
                {

                    case SpellTarget.CIRCLE:
                    case SpellTarget.AURA:
                        bodyAnimator.SetTrigger("CastSpell");

                        spellTarget.GetComponent<MagicTargetBase>().Cast();
                        break;

                    case SpellTarget.MISSILE:

                        //bodyAnimator.SetTrigger("CastSpell");
                        var missileTarget = spellTarget.GetComponent<MagicTargetMissileController>();

                        if (specialCast)
                        {
                            missileTarget.Select();
                        }
                        else
                        {
                            bodyAnimator.SetTrigger("CastSpell");
                            missileTarget.Cast();
                        }
                        break;
                }

            }

        }



        // Use this for initialization
        void Start()
        {


            selectedSpell.Subscribe(spell =>
            {

                if (spellTarget != null)
                {
                    Destroy(spellTarget);
                    spellTarget = null;
                }

                if (spell == null)
                    return;

                switch (spell.spellTarget)
                {
                    case SpellTarget.CIRCLE:
                    case SpellTarget.AURA:

                        spellTarget = Instantiate(
                            circleTargetPrefab);

                        var circleTarget = spellTarget.GetComponent<MagicTargetCircleController>();

                        circleTarget.targetPrefab = circleTargetPrefab;
                        circleTarget.caster = this;


                        circleTarget.effects = new List<GameObject>() { circleTargetEffectPrefab }.Select(x =>
                        {
                            var effectCopy = Instantiate(x);
                            effectCopy.transform.SetParent(circleTarget.transform);
                            return effectCopy.GetComponent<ParticleSystem>();
                        }).ToList();


                        circleTarget.follow = spell.spellTarget == SpellTarget.CIRCLE ? targetTransform : transform;
                        circleTarget.radius = 2;

                        break;
                    case SpellTarget.MISSILE:

                        spellTarget = Instantiate(missileTargetPrefab);

                        var missileTarget = spellTarget.GetComponent<MagicTargetMissileController>();

                        missileTarget.follow = targetTransform;
                        missileTarget.caster = this;
                        missileTarget.targetPrefab = missileTargetPrefab;

                        missileTarget.maxTargets = spell.maxTargets;

                        break;

                }
            });

            currentMana.Value = maxMana.Value;
        }

        // Update is called once per frame
        void Update()
        {

            if (spellTarget != null)
            {
                switch (selectedSpell.Value.spellTarget)
                {
                    case SpellTarget.CIRCLE:
                    case SpellTarget.AURA:

                        var magicCircleTarget = spellTarget.GetComponent<MagicTargetCircleController>();

                        if (spellRadius != magicCircleTarget.radius)
                        {
                            // update effect radiuses

                            magicCircleTarget.setRadius(
                               spellRadius
                            );

                        }

                        break;
                }
            }


        }
    }
}
