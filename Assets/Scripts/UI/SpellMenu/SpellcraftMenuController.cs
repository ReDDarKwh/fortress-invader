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
            craftZone.onDragEnter.AddListener(AddSpellComponentToCraftZone);

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
                var draggable = component.GetComponent<Draggable>();
                draggable.data = c;
            }
        }

        private void AddSpellComponentToCraftZone()
        {
            //throw new NotImplementedException();
        }

        private void RemoveSpellComponentFromCraftZone(Draggable draggable)
        {
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

