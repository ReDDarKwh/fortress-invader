using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Characters;
using UnityEngine;
using UnityEngine.Events;

public partial class StateMachine : MonoBehaviour
{


    private Dictionary<BaseState, List<StateMachineEventWithActiveLinking>> UpdateSubs = new Dictionary<BaseState, List<StateMachineEventWithActiveLinking>>();
    private Dictionary<BaseState, List<UnityEvent>> LeaveSubs = new Dictionary<BaseState, List<UnityEvent>>();
    private Dictionary<BaseState, List<StateMachineEventWithActiveLinking>> EnterSubs = new Dictionary<BaseState, List<StateMachineEventWithActiveLinking>>();

    public BaseState entryState;
    public List<BaseEventStateLinker> stateLinkers;

    private Dictionary<BaseState, ActiveLinking> activeStates =
     new Dictionary<BaseState, ActiveLinking>();



    public bool debugShowStates = false;

    public IEnumerable<BaseState> GetActiveStates()
    {
        return activeStates.Keys.ToList();
    }

    public void Update()
    {
        var pendingActions = new Queue<Tuple<BaseState, EventStateLinking>>();

        if (activeStates.Count < 1)
        {
            StartState(this.entryState, null, false);
        }

        foreach (var linking in activeStates)
        {
            foreach (var link in linking.Value.links)
            {
                EventMessage message;
                if (link.invert ?
                    !link.triggeredOn.Check(this.gameObject, linking.Value, out message) :
                    link.triggeredOn.Check(this.gameObject, linking.Value, out message)
                )
                {
                    link.eventResponse = message;
                    pendingActions.Enqueue(new Tuple<BaseState, EventStateLinking>(linking.Key, link));

                }
            }
        }

        // update the active states
        foreach (var state in activeStates.Keys)
        {
            state.StateUpdate(this, activeStates[state]);
            AfterUpdate(activeStates[state]);
        }

        while (pendingActions.Count > 0)
        {
            var action = pendingActions.Dequeue();
            ExecuteEventAction(action.Item1, action.Item2);
        }

    }


    public void AddSubscription(StateMachineSubscription subscription)
    {
        if (subscription.onStateEnter != null)
        {

            if (!EnterSubs.ContainsKey(subscription.state))
            {
                EnterSubs[subscription.state] = new List<StateMachineEventWithActiveLinking>();
            }

            EnterSubs[subscription.state].Add(subscription.onStateEnter);

        }
        if (subscription.onStateUpdate != null)
        {
            if (!UpdateSubs.ContainsKey(subscription.state))
            {
                UpdateSubs[subscription.state] = new List<StateMachineEventWithActiveLinking>();
            }

            UpdateSubs[subscription.state].Add(subscription.onStateUpdate);
        }
        if (subscription.onStateLeave != null)
        {
            if (!LeaveSubs.ContainsKey(subscription.state))
            {
                LeaveSubs[subscription.state] = new List<UnityEvent>();
            }

            LeaveSubs[subscription.state].Add(subscription.onStateLeave);
        }
    }

    public void AfterUpdate(ActiveLinking linking)
    {

        if (!UpdateSubs.ContainsKey(linking.state))
            return;
        foreach (var sub in UpdateSubs[linking.state])
        {
            sub.Invoke(linking);
        }
    }

    public void AfterEnter(ActiveLinking linking)
    {
        if (!EnterSubs.ContainsKey(linking.state))
            return;
        foreach (var sub in EnterSubs[linking.state])
        {
            sub.Invoke(linking);
        }
    }
    public void AfterLeave(BaseState state)
    {
        if (!LeaveSubs.ContainsKey(state))
            return;
        foreach (var sub in LeaveSubs[state])
        {
            sub.Invoke();
        }
    }


    // find the first event with the same name in the active state and trigger it
    public void TriggerEvent(BaseEvent e, EventMessage message)
    {

        var pendingActions = new Queue<Tuple<BaseState, EventStateLinking>>();
        foreach (var linking in activeStates)
        {
            foreach (var link in linking.Value.links)
            {
                if (link.triggeredOn.name == e.name + "(Clone)")
                {
                    link.eventResponse = message;
                    pendingActions.Enqueue(new Tuple<BaseState, EventStateLinking>(linking.Value.state, link));
                }
            }
        }

        while (pendingActions.Count > 0)
        {
            var action = pendingActions.Dequeue();
            ExecuteEventAction(action.Item1, action.Item2);
        }

    }

    private void ExecuteEventAction(BaseState fromState, EventStateLinking e)
    {
        switch (e.action.actionType)
        {
            case EventActionType.TRANSITION:
            case EventActionType.TRANSITION_OVERRIDE:

                EndState(fromState);

                if (e.action.transitionToState != null)
                    StartState(e.action.transitionToState, e, e.action.actionType == EventActionType.TRANSITION_OVERRIDE);

                foreach (var s in e.action.addStates)
                {
                    StartState(s, e, e.action.actionType == EventActionType.TRANSITION_OVERRIDE);
                }
                break;

            case EventActionType.REMOVE:
                EndState(fromState);
                break;

            case EventActionType.ADD:
            case EventActionType.ADD_OVERRIDE:

                if (e.action.transitionToState != null)
                    StartState(e.action.transitionToState, e, e.action.actionType == EventActionType.ADD_OVERRIDE);
                foreach (var s in e.action.addStates)
                {
                    StartState(s, e, e.action.actionType == EventActionType.ADD_OVERRIDE);
                }
                break;
        }
    }


    private List<EventStateLinking> GetLinkingsForState(BaseState state)
    {
        return this.stateLinkers
                .Select(x => x.GetLinksForState(state.GetTagsPlusName())
                .ToDictionary(y => new { y.tagName, y.triggeredOn })).Aggregate((a, b) =>
                {
                    foreach (var link in b)
                    {
                        if (a.ContainsKey(link.Key))
                        {
                            a[link.Key] = link.Value;
                        }
                        else
                        {
                            a.Add(link.Key, link.Value);
                        }
                    }
                    return a;
                }).Values.Select(x => new EventStateLinking()
                {
                    action = x.action,
                    triggeredOn = Instantiate(x.triggeredOn),
                    invert = x.invert
                }).ToList();
    }


    public void StartState(BaseState stateToSwitchTo, EventStateLinking e, bool doAnOverride)
    {

        // if state already running override or cancel
        if (activeStates.ContainsKey(stateToSwitchTo))
        {
            if (doAnOverride)
            {
                EndState(stateToSwitchTo);
            }
            else
            {
                return;
            }
        }

        activeStates[stateToSwitchTo] = new ActiveLinking()
        {
            links = GetLinkingsForState(stateToSwitchTo),
            linkingProperties = new Dictionary<string, object>(),
            timeStarted = Time.time,
            state = stateToSwitchTo
        };

        //start state that will fill the linkingProperties
        stateToSwitchTo.Enter(this, e?.eventResponse, activeStates[stateToSwitchTo]);

        AfterEnter(activeStates[stateToSwitchTo]);

        //give linking properties to each events to convert to event properties
        foreach (var link in activeStates[stateToSwitchTo].links)
        {
            link.triggeredOn.Init(activeStates[stateToSwitchTo]);
        }
    }

    private void EndState(BaseState state)
    {
        //remove every links that affect the state

        if (activeStates.ContainsKey(state))
        {
            activeStates.Remove(state);
            state.Leave(this);
            AfterLeave(state);
        }
    }
}

