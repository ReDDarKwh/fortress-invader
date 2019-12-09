




using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System.Linq;


namespace Scripts.Spells
{

    public class Spell : SpellBase
    {
        public IEnumerable<SpellEffectContainer> spellEffects;

        public static SavedSpell ToSavedSpell(Spell spell)
        {

            var result = new SavedSpell
            {
                spellName = spell.spellName,
                spellEffects = spell.spellEffects
                .Select(x => new SavedSpellEffectContainer(x.spellEffect.effectName, x.position))
                .ToList(),
                spellTarget = spell.spellTarget,
                spellTargetPosition = spell.spellTargetPosition,
                createCost = spell.createCost,
                manaCost = spell.manaCost,
                travelCost = spell.travelCost,
                maxRadius = spell.maxRadius,
                maxTargets = spell.maxTargets
            };
            return result;
        }

        public static Spell FromSavedSpell(SavedSpell spell, IEnumerable<SpellEffect> spellEffects)
        {
            var effects = spellEffects.ToDictionary(x => x.effectName);
            return new Spell
            {
                spellName = spell.spellName,
                spellEffects = spell.spellEffects.Select(x => new SpellEffectContainer
                {
                    spellEffect = effects[x.item1],
                    position = x.item2
                }).ToList(),
                spellTarget = spell.spellTarget,
                spellTargetPosition = spell.spellTargetPosition,
                createCost = spell.createCost,
                manaCost = spell.manaCost,
                travelCost = spell.travelCost,
                maxRadius = spell.maxRadius,
                maxTargets = spell.maxTargets
            };
        }
        public Spell()
        {
            spellEffects = new List<SpellEffectContainer>();
        }
    }
}

