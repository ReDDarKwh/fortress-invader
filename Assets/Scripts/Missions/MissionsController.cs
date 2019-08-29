using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace Scripts.Missions{
    public class MissionsController : MonoBehaviour
    {
        public List<Mission> missions;
        public ReactiveProperty<Mission> selectedMission;
        public int numberOfMissions;

        public int minNumberOfOperationsPerMission = 1;
        public int maxNumberOfOperationsPerMission = 5;
         
        private readonly Type[] objectiveTypes = new Type[]{
            typeof(KillObjective)
        };

        public void GenerateMissions(){

            missions = new List<Mission>();

            for(var i = 0; i< numberOfMissions; i++){

                var mission = new Mission();

                for(var j = 0; j < UnityEngine.Random.Range(minNumberOfOperationsPerMission, maxNumberOfOperationsPerMission); j++){

                    var objective = ((Objective)Activator.CreateInstance(
                            objectiveTypes[UnityEngine.Random.Range(0,objectiveTypes.Length)]
                        ));
                    
                    objective.difficulty = (MissionDifficulty)UnityEngine.Random.Range(0,5);

                    objective.Init();

                    ((List<Objective>)mission.objective).Add(
                        objective
                    );
                }

                missions.Add(
                  mission
                );
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
