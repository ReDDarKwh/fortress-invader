using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Spells;
using UnityEngine;


namespace Scripts.UI
{

    public class SpellcraftMenuController : SpellMenu
    {

        public DropZone effectZone;
        public DropZone targetZone;
        public DropZone modifierZone;
        public DropZone craftZone;

        public List<SpellEffect> effects;

        public GameObject effectComponentItemPrefab;

        public GameObject targetComponentItemPrefab;

        // Start is called before the first frame update
        void Start()
        {
            craftZone.onDragFailed.AddListener(RemoveSpellComponentFromCraftZone);
            craftZone.onDrop.AddListener(AddSpellComponentToCraftZone);

            // add effects
            CreateComponents(effectComponentItemPrefab, effects, effectZone);
            // add targets
            CreateComponents(targetComponentItemPrefab, Enum.GetNames(typeof(SpellTarget)), targetZone);
        }

        private void CreateComponents(GameObject prefab, IEnumerable<object> components, DropZone zone)
        {
            foreach (var c in components)
            {
                var component = Instantiate(prefab, zone.transform);

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

            Debug.Log(draggable.data);

            //throw new NotImplementedException();
        }

        public void RemoveSpellComponentFromCraftZone(Draggable draggable)
        {


            Debug.Log(draggable.data);


            if (draggable.data is SpellEffect)
            {
                draggable.AddDraggableToZone(effectZone);

            }
            else if (draggable.data is SpellTarget)
            {
                draggable.AddDraggableToZone(targetZone);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }


        protected override void BeforeClose()
        {
        }

        protected override void BeforeOpen()
        {
        }

    }


}

