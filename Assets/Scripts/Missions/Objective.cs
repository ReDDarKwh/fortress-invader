using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Scripts.Missions{
    public abstract class Objective 
    {
        public MissionDifficulty difficulty;
        public string description;
        public abstract void Init();
    }
}
