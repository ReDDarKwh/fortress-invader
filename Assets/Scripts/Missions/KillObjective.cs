using System.Collections;
using System.Collections.Generic;
using Scripts.NPC;
using UnityEngine;



namespace Scripts.Missions
{
    public class KillObjective : Objective
    {
        public KillObjectiveType killObjectiveType;
        // can be used has the exact target
        public NonPlayerCharacter target;

        public KillObjective(NonPlayerCharacter target)
        {
            this.target = target;
            // TODO : choas must be set based on the difficulty of the operation. target difficulty score?
            chaos = target.difficultyRating;
            desc = $"Kill the target named : {target.role} {target?.characterName}";
        }

        internal override void Enter(BaseState targetState)
        {
            var sm = target.GetComponent<StateMachine>();
            sm.StartState(targetState, null, true);
        }

        internal override void Leave(BaseState targetState)
        {
            var sm = target.GetComponent<StateMachine>();
            sm.EndState(targetState);
        }

        public enum KillObjectiveType
        {
            Group,
            SingleTarget
        }
    }
}