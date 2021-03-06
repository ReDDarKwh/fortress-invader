﻿using System.Collections;
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

        public KillObjective(NonPlayerCharacter target, BaseState deadState)
        {
            this.target = target;
            var stateMachine = this.target.GetComponent<StateMachine>();
            var onDeadEvent = new StateMachineEventWithActiveLinking();

            onDeadEvent.AddListener(HandleDone);

            stateMachine.AddSubscription(new StateMachineSubscription
            {
                state = deadState,
                stateMachine = stateMachine,
                onStateEnter = onDeadEvent
            });

            // TODO : choas must be set based on the difficulty of the operation. target difficulty score?
            chaos = target.difficultyRating * 10;
            desc = $"Kill the target named : {target.role} {target?.characterName}";
        }

        void HandleDone(ActiveLinking linking)
        {
            done.Value = true;
        }

        internal override void Enter(BaseState targetState)
        {
            var sm = target.GetComponent<StateMachine>();
            sm.StartState(targetState, null, true);
        }

        internal override Vector3 GetPosition()
        {
            return target.transform.position;
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