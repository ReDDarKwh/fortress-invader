using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using Scripts.Spells;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

namespace Scripts.UI
{
    public class SpellbookMenuController : SpellMenu
    {

        public GameObject spellElementPrefab;
        public GameObject spellbookContainerElement;
        public EffectContainer ec;

        void Start()
        {
            BeforeOpen();
        }

        protected override void BeforeClose()
        {

            foreach (Transform child in spellbookContainerElement.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

        }
        protected override void BeforeOpen()
        {

            FileInfo[] files = SaveGame.GetFiles("spells");

            foreach (var spell in files.Select(f=>{
                return  new {spell = SaveGame.Load<SavedSpell>("spells/" + f.Name), id = f.Name};
            }).OrderBy(x=>x.spell.spellName))
            {
                Debug.Log(spell.spell.spellName);

                var spellElement = Instantiate(spellElementPrefab, spellbookContainerElement.transform)
                .GetComponent<SpellUIElementController>();
                spellElement.spell = Spell.FromSavedSpell(spell.spell, ec.effects);
                var button = spellElement.GetComponent<Button>();
                button.onClick.AddListener(delegate
                {
                    this.uiController.OpenSpellcraftMenu(spell.id);
                });
            }
        }



    }
}

