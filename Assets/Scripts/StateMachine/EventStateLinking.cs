

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Characters
{

    [System.Serializable]
    public class EventStateLinking
    {

        [System.NonSerialized]
        public string tagName; // state name or tag.

        public List<BaseState> states;

        public BaseEvent triggeredOn;
        public bool invert = false;
        public EventAction action;


        [System.NonSerialized]
        public EventMessage eventResponse;

    }

}