using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using Scripts.Spells;
using System.IO;
using System;

namespace Scripts.UI
{


    public class SpellbookUIController : MonoBehaviour
    {


        // public List<Spell> spellbook;
        // public SpellEffect spellEffect;

        public SpellcraftMenuController spellcraftMenu;
        public SpellbookMenuController spellbookMenu;

        private void CloseMenus(SpellMenu menu)
        {
            menu.Close();
        }

        public void OpenSpellcraftMenu(string spellId)
        {
            spellbookMenu.Close();
            spellcraftMenu.spellID =  String.IsNullOrEmpty(spellId)? Guid.Empty : Guid.Parse(spellId);
            spellcraftMenu.Open();
        }

        public void OpenSpellbookMenu()
        {
            spellcraftMenu.Close();
            spellbookMenu.Open();
        }


        // Start is called before the first frame update
        void Start()
        {

            // Debug.Log(spellbook);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

