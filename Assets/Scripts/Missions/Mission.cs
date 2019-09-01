
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UI;

namespace Scripts.Missions
{
    public class Mission
    {
        public MissionDifficulty difficulty;
        public ReactiveProperty<Objective> currentObjective = new ReactiveProperty<Objective>();
        public IEnumerable<Objective> objectives = new List<Objective>();
        public string missionName;
        public string desc;
        public float chaos;
        public bool selected;
    }
}
