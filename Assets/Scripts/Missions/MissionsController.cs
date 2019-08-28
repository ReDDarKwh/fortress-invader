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

        private readonly IEnumerable<Type> objectiveTypes = new List<Type>{
            typeof(KillObjective)
        };

        void GenerateMissions(int numberOfMissions){

            

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
