using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace Scripts.Missions{
    public class MissionsController : MonoBehaviour
    {
        public List<Mission> missions;

        public ReactiveProperty<Mission> selectedMission;


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
