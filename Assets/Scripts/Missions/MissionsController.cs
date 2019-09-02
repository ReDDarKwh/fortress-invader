﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using Scripts.NPC;

namespace Scripts.Missions
{
    public class MissionsController : MonoBehaviour
    {
        public List<Mission> missions;
        public ReactiveProperty<Mission> selectedMission = new ReactiveProperty<Mission>();
        internal ReactiveProperty<Mission> lastSelectedMission = new ReactiveProperty<Mission>();
        public int numberOfMissions;

        public int minNumberOfOperationsPerMission = 1;
        public int maxNumberOfOperationsPerMission = 5;


        public Queue<Objective> objectiveBank = new Queue<Objective>();
        public BaseState targetState;

        private readonly Type[] objectiveTypes = new Type[]{
            typeof(KillObjective)
        };

        public List<Vector3> GetCurrentObjectivePositions()
        {
            var results = new List<Vector3>();
            var pos = selectedMission.Value?.currentObjective.Value?.GetPosition();

            if (pos != null)
            {
                results.Add(pos.Value);
            }
            return results;
        }

        public void GenerateMissions()
        {

            missions = new List<Mission>();

            for (var i = 0; i < numberOfMissions; i++)
            {
                List<Objective> objectives = new List<Objective>();

                for (var j = 0; j < UnityEngine.Random.Range(minNumberOfOperationsPerMission, maxNumberOfOperationsPerMission); j++)
                {
                    if (objectiveBank.Count() > 0)
                    {
                        objectives.Add(objectiveBank.Dequeue());
                    }
                }

                if (objectives.Count() <= 0)
                {
                    break;
                }

                var mission = new Mission();

                mission.objectives = objectives;

                mission.missionName = $"Mission #{i + 1}";

                mission.chaos = mission.objectives.Sum(x => x.chaos);

                mission.difficulty = (MissionDifficulty)Mathf.Round(mission.chaos.Remap(0, 20, 0, 5));

                mission.currentObjective.Value = mission.objectives.First();

                mission.desc = $"Mission has {mission.objectives.Count()} step{(mission.objectives.Count() > 1 ? "s" : "")}. \n" +
                $"<b>Current objective : </b>{mission.currentObjective.Value.desc}";

                missions.Add(mission);
            }
        }


        public void ScanForMissions()
        {
            var characters = GameObject.FindObjectsOfType<NonPlayerCharacter>();
            foreach (var c in characters)
            {
                if (UnityEngine.Random.value < 0.3 + c.difficultyRating)
                {
                    objectiveBank.Enqueue(new KillObjective(c));
                }
            }

            GenerateMissions();
        }


        // Start is called before the first frame update
        void Start()
        {
            lastSelectedMission.Subscribe((x) =>
            {
                x?.currentObjective.Value?.Leave(targetState);
            });
            selectedMission.Subscribe((x) =>
            {
                x?.currentObjective.Value?.Enter(targetState);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
