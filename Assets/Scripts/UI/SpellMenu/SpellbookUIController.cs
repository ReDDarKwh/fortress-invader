using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;
using Scripts.Spells;



namespace Scripts.UI
{


    public class SpellbookUIController : MonoBehaviour
    {

        // public GameObject spellElementPrefab;
        // public GameObject spellbookContainerElement;
        // public List<Spell> spellbook;
        // public SpellEffect spellEffect;

        public SpellcraftMenuController spellcraftMenu;
        public SpellbookMenuController spellbookMenu;

        private void CloseMenus(SpellMenu menu)
        {
            menu.Close();
        }

        public void OpenSpellcraftMenu()
        {
            spellbookMenu.Close();
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
            // spellbook = SaveGame.Exists("spellbook") ?
            //     SaveGame.Load<List<Spell>>("spellbook") : new List<Spell>();

            // foreach (var spell in spellbook)
            // {
            //     var spellElement = Instantiate(spellElementPrefab, spellbookContainerElement.transform)
            //     .GetComponent<SpellUIElementController>();
            //     spellElement.spell = spell;
            // }

            // Debug.Log(spellbook);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

