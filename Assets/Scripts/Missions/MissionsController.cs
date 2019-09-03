using System.Collections;
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
        public ReactiveProperty<List<Mission>> missions = new ReactiveProperty<List<Mission>>();
        public ReactiveProperty<Mission> selectedMission = new ReactiveProperty<Mission>();
        internal ReactiveProperty<Mission> lastSelectedMission = new ReactiveProperty<Mission>();
        public int numberOfMissions;
        public int minNumberOfOperationsPerMission = 1;
        public int maxNumberOfOperationsPerMission = 5;
        public Queue<Objective> objectiveBank = new Queue<Objective>();
        public BaseState targetState;
        public BaseState deadState;
        public FloatReactiveProperty chaos = new FloatReactiveProperty();

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
            var missions = new List<Mission>();
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

                var mission = new Mission(
                    objectives: objectives,
                    missionName: $"Mission #{i + 1}"
                    );

                missions.Add(mission);
            }
            this.missions.Value = missions;
        }
        public void ScanForMissions()
        {
            var characters = GameObject.FindObjectsOfType<NonPlayerCharacter>();
            foreach (var c in characters)
            {
                if (UnityEngine.Random.value < 0.3 + c.difficultyRating)
                {
                    objectiveBank.Enqueue(new KillObjective(c, deadState));
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
