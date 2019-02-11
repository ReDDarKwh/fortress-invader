


using Scripts.NPC;
using UnityEngine;
using System.Collections.Generic;

namespace Scripts.NPC
{

    [CreateAssetMenu(fileName = "SurprisedState", menuName = "StateMachine/States/Enemy/SurprisedState")]
    public class SurprisedState : BaseState
    {
        public SurprisedState()
        {
            this.stateName = "surprised";
        }
        override public void Leave(StateMachine stateMachine)
        {
        }
        override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
        {

        }
        override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
        {
        }
    }

}


