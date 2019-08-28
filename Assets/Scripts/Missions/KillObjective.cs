using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Scripts.Missions{
    public class KillObjective : Objective
    {
        public KillObjectiveType killObjectiveType;

        // can be used has the exact target
        public Character target;

        public override void Init()
        {
            throw new System.NotImplementedException();
        }

        public enum KillObjectiveType
        {
            Group,
            SingleTarget
        }
    }
}