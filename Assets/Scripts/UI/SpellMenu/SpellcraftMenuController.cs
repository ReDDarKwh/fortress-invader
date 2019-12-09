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
        public EffectContainer ec;

        public GameObject effectComponentItemPrefab;

        public GameObject targetComponentItemPrefab;

        private Spell spell;

        public Guid spellID;

        public string saveFilePrefix = "spells/";

        // Start is called before the first frame update
        void Start()
        {
            craftZone.onDragFailed.AddListener(RemoveSpellComponentFromCraftZone);
            craftZone.onDrop.AddListener(AddSpellComponentToCraftZone);
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

                var index = ((List<SpellEffectContainer>)spell.spellEffects).FindIndex(x => x.spellEffect.effectName == effect.effectName);

                if (index == -1)
                {
                    ((List<SpellEffectContainer>)spell.spellEffects).Add(
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
                spell.spellTargetPosition = draggable.GetComponent<RectTransform>().anchoredPosition;
                RefreshCraftZone();
            }

            Save();
        }

        public void RemoveSpellComponentFromCraftZone(Draggable draggable)
        {
            if (draggable.data is SpellEffect)
            {
                draggable.AddDraggableToZone(effectZone);

                var effect = (SpellEffect)draggable.data;
                var index = ((List<SpellEffectContainer>)spell.spellEffects).FindIndex(x => x.spellEffect.effectName == effect.effectName);
                if (index != -1)
                {
                    ((List<SpellEffectContainer>)spell.spellEffects).RemoveAt(index);
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
            ClearCraftZone();
        }

        private void RefreshCraftZone(){
            ClearCraftZone();
            FillCraftZone();
        }

        private void ClearCraftZone()
        {
            // clear craftZone;
            foreach (Transform child in craftZone.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in effectZone.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in targetZone.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void FillCraftZone()
        {

            foreach (var effectContainer in spell.spellEffects)
            {
                CreateComponent(effectComponentItemPrefab, effectContainer.spellEffect, craftZone, effectContainer.position);
            }

            if (spell.spellTarget != 0)
            {
                CreateComponent(targetComponentItemPrefab, spell.spellTarget.ToString(), craftZone, spell.spellTargetPosition);
            }

            var usedEffets = spell.spellEffects.Select(x => x.spellEffect);
            // add effects
            CreateComponents(effectComponentItemPrefab, ec.effects.Where(x => !usedEffets.Contains(x)), effectZone, Vector3.zero);
            // add targets
            CreateComponents(targetComponentItemPrefab, Enum.GetNames(typeof(SpellTarget))
            .Where(x => x != spell.spellTarget.ToString()), targetZone, Vector3.zero);

        }

        private void Save()
        {
            var spellPath = saveFilePrefix + spellID;
            SaveGame.Save<SavedSpell>(spellPath, Spell.ToSavedSpell(spell));
        }

        public void RemoveSpell()
        {
            var spellPath = saveFilePrefix + spellID;
            SaveGame.Delete(spellPath);
        }

        protected override void BeforeOpen()
        {
            // new spell;
            if (spellID == null || spellID == Guid.Empty)
            {
                spellID = Guid.NewGuid();
            }

            var spellPath = saveFilePrefix + spellID;
            var spellExists = SaveGame.Exists(spellPath);

            spell = spellExists ?
            Spell.FromSavedSpell(SaveGame.Load<SavedSpell>(spellPath), ec.effects) : new Spell();

            spellNameInput.text = spell.spellName;
            FillCraftZone();
        }

        public void SpellNameChanged(string value)
        {
            spell.spellName = value;
            Save();
        }

    }


}

