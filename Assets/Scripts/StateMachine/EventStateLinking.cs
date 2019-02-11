

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Characters
{

    [System.Serializable]
    public class EventStateLinking
    {

        [Tooltip("Name or tag of the states(s) that this linker will affect")]
        public string tagName; // state name or tag.
        public BaseEvent triggeredOn;
        public bool invert = false;
        public EventAction action;


        [System.NonSerialized]
        public EventMessage eventResponse;

    }

}