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

        // Start is called before the first frame update
        void Start()
        {
            craftZone.onDragFailed.AddListener(RemoveSpellComponentFromCraftZone);
            foreach (var e in effects)
            {
                var effectComponent = Instantiate(effectComponentItemPrefab, effectZone.transform);
                var draggable = effectComponent.GetComponent<Draggable>();
                draggable.data = e;
            }
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

