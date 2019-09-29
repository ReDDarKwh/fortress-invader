using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using Scripts.Spells;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
namespace Scripts.UI
{

    public class SpellcraftMenuController : SpellMenu
    {

        public DropZone effectZone;
        public DropZone targetZone;
        public DropZone modifierZone;
        public DropZone craftZone;
        public InputField spellNameInput;
        public List<SpellEffect> effects;

        public GameObject effectComponentItemPrefab;

        public GameObject targetComponentItemPrefab;

        private Spell spell;

        public Guid spellID = Guid.Parse("c8bd0b35-0bbc-49ef-8207-1644917b82f9");

        public string saveFilePrefix = "spells/";

        // Start is called before the first frame update
        void Start()
        {
            craftZone.onDragFailed.AddListener(RemoveSpellComponentFromCraftZone);
            craftZone.onDrop.AddListener(AddSpellComponentToCraftZone);

            // add effects
            CreateComponents(effectComponentItemPrefab, effects, effectZone, Vector3.zero);
            // add targets
            CreateComponents(targetComponentItemPrefab, Enum.GetNames(typeof(SpellTarget)), targetZone, Vector3.zero);

            BeforeOpen();
        }

        private void CreateComponent(GameObject prefab, object component, DropZone zone, Vector3 position)
        {
            CreateComponents(prefab, new List<object> { component }, zone, position);
        }

        private void CreateComponents(GameObject prefab, IEnumerable<object> components, DropZone zone, Vector3 position)
        {
            foreach (var c in components)
            {
                var component = Instantiate(prefab, zone.transform);

                component.GetComponent<RectTransform>().anchoredPosition = position;

                var componentUI = component.GetComponent<SpellComponentUIController>();

                if (c is string)
                {
                    componentUI.nameText.text = (string)c;
                }
                else if (c is SpellEffect)
                {
                    componentUI.nameText.text = ((SpellEffect)c).effectName;
                    componentUI.logoText.text = ((SpellEffect)c).effectName.Substring(0, 2).ToUpper();

                }

                var draggable = component.GetComponent<Draggable>();
                draggable.data = c;
            }
        }

        public void AddSpellComponentToCraftZone(Draggable draggable)
        {

            if (draggable.data is SpellEffect)
            {
                var effect = (SpellEffect)draggable.data;

                var index = spell.spellEffects.FindIndex(x => x.spellEffect.effectName == effect.effectName);

                if (index == -1)
                {
                    spell.spellEffects.Add(
                        new SpellEffectContainer
                        {
                            spellEffect = effect,
                            position = draggable.GetComponent<RectTransform>().anchoredPosition
                        }
                    );
                }
                else
                {
                    spell.spellEffects.ElementAt(index).position = draggable.GetComponent<RectTransform>().anchoredPosition;
                }
            }
            else if (draggable.data is string)
            {
                Enum.TryParse<SpellTarget>((string)draggable.data, out SpellTarget target);
                spell.spellTarget = target;
            }

            Save();
        }

        public void RemoveSpellComponentFromCraftZone(Draggable draggable)
        {
            if (draggable.data is SpellEffect)
            {
                draggable.AddDraggableToZone(effectZone);

                var effect = (SpellEffect)draggable.data;
                var index = spell.spellEffects.FindIndex(x => x.spellEffect.effectName == effect.effectName);
                if (index != -1)
                {
                    spell.spellEffects.RemoveAt(index);
                }
            }
            else if (draggable.data is string)
            {
                draggable.AddDraggableToZone(targetZone);
            }

            Save();
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void BeforeClose()
        {
            //Save();
        }
        private void Save()
        {
            var spellPath = saveFilePrefix + spellID;
            SaveGame.Save<Spell>(spellPath, spell);
        }

        protected override void BeforeOpen()
        {
            // new spell;
            if (spellID == Guid.Empty)
            {
                spellID = Guid.NewGuid();
            }

            var spellPath = saveFilePrefix + spellID;
            var spellExists = SaveGame.Exists(spellPath);

            spell = spellExists ?
            SaveGame.Load<Spell>(spellPath) : new Spell();

            spellNameInput.text = spell.spellName;

            foreach (var effectContainer in spell.spellEffects)
            {
                CreateComponent(effectComponentItemPrefab, effectContainer.spellEffect, craftZone, effectContainer.position);
            }


        }

        public void SpellNameChanged(string value)
        {

            spell.spellName = value;
            Save();
        }

    }


}

