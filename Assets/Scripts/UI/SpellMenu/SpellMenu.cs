

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.UI
{
    public abstract class SpellMenu : MonoBehaviour
    {

        public SpellbookUIController uiController;

        protected abstract void BeforeClose();

        protected abstract void BeforeOpen();


        internal void Close()
        {
            BeforeClose();
            this.gameObject.SetActive(false);
        }

        internal void Open()
        {
            BeforeOpen();
            this.gameObject.SetActive(true);
        }
    }
}