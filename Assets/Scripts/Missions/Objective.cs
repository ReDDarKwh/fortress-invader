using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;



namespace Scripts.Missions
{
    public abstract class Objective
    {
        public float chaos;
        public string desc;
        public BoolReactiveProperty done = new BoolReactiveProperty(false);
        internal abstract Vector3 GetPosition();
        internal abstract void Enter(BaseState targetState);
        internal abstract void Leave(BaseState targetState);

    }
}
