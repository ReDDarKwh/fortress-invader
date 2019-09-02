
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

namespace Scripts.Missions
{
    public class Mission
    {
        public MissionDifficulty difficulty;
        public ReactiveProperty<Objective> currentObjective = new ReactiveProperty<Objective>();
        public IEnumerable<Objective> objectives = new List<Objective>();
        public string missionName;
        public string desc;
        public IObservable<bool> done;
        public float chaos;
        public bool selected;

        public Mission(List<Objective> objectives, string missionName)
        {
            this.objectives = objectives;
            this.missionName = missionName;

            chaos = this.objectives.Sum(x => x.chaos);
            difficulty = (MissionDifficulty)Mathf.Round(chaos.Remap(0, 20, 0, 5));

            currentObjective.Value = objectives.First();

            desc = $"Mission has {objectives.Count()} step{(objectives.Count() > 1 ? "s" : "")}. \n" +
            $"<b>Current objective : </b>{currentObjective.Value.desc}";

            done = objectives.Select(x => x.done).CombineLatestValuesAreAllTrue();
        }
    }
}
