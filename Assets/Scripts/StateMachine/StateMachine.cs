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
    private LinkRepo stateLinkers;
    private Dictionary<BaseState, ActiveLinking> activeStates =
        new Dictionary<BaseState, ActiveLinking>();

    public BaseState entryState;
    public List<SMGraph> stateMachineGraphs;
    public bool debugShowStates = false;
    public IEnumerable<BaseState> GetActiveStates()
    {
        return activeStates.Keys.ToList();
    }

    public void Start()
    {
        stateLinkers = GenerateLinkRepo(stateMachineGraphs);
    }

    public void Update()
    {
        var pendingActions = new Queue<Tuple<BaseState, EventStateLinking>>();

        if (activeStates.Count < 1 && stateLinkers != null)
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

    // find the events with the same name in the active states and trigger them
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


    private void clearActiveState()
    {
        foreach (var entry in activeStates)
        {
            foreach (var l in entry.Value.links)
            {
                Destroy(l.triggeredOn);
            }
            EndState(entry.Value.state);
        }
    }




    private class LinkRepo
    {
        public Dictionary<BaseState, IEnumerable<EventStateLinking>> LinksByState { get; set; }
        public Dictionary<string, IEnumerable<EventStateLinking>> LinksByTag { get; set; }
    }



    private class EventLinkingGroups
    {
        public IEnumerable<BaseState> states { get; set; }
        public IEnumerable<string> tagnames { get; set; }
        public string triggerName { get; set; }
        public EventActionType actionType { get; set; }

    }

    class LinkerGroupEqualityComparer : IEqualityComparer<EventLinkingGroups>
    {
        public bool Equals(EventLinkingGroups g1, EventLinkingGroups g2)
        {
            return g1.states.Select(x => x.stateName).SequenceEqual(g2.states.Select(x => x.stateName)) &&
            g1.tagnames.SequenceEqual(g2.tagnames) &&
            g1.actionType.Equals(g2.actionType) &&
            g1.triggerName.Equals(g2.triggerName);
        }

        public int GetHashCode(EventLinkingGroups g)
        {
            return
            g.actionType.GetHashCode() ^
            g.states.Select(x => x.stateName).Aggregate(0, (a, b) => a ^ b.GetHashCode()) ^
            g.tagnames.Aggregate(0, (a, b) => a ^ b.GetHashCode()) ^
            g.triggerName.GetHashCode();
        }
    }



    private LinkRepo GenerateLinkRepo(List<SMGraph> stateMachineGraphs)
    {
        // triggers need to be instantiated to work. 
        // Remaking the link dictionnary requires to remove previously created triggers.
        // else memory leak
        clearActiveState();
        var groups =
         stateMachineGraphs
         .SelectMany(x =>

             x.nodes
                .Where(n => n is LinkNode)
                .Select(n => n as LinkNode)
                .Select(n => new EventStateLinking
                {

                    states = n.GetInputValues<IEnumerable<BaseState>>("from", new List<BaseState>())
                    .SelectMany(p => p),
                    tagNames = n.GetInputValue<IEnumerable<string>>("tags"),

                    triggeredOn = Instantiate(n.trigger),
                    invert = n.invert,
                    action = new EventAction
                    {
                        actionType = n.actionType,
                        addStates = n.Outputs
                        .ElementAt(0)
                        .GetConnections()
                        .SelectMany(c => (c.node as StateNode).states)
                    }
                })
        )

        // only take the last version of same linker between graphs (so we get overrides)
        .GroupBy(x => new EventLinkingGroups
        {
            states = x.states ?? new List<BaseState>(),
            tagnames = x.tagNames ?? new List<string>(),
            triggerName = x.triggeredOn.name,
            actionType = x.action.actionType
        }, new LinkerGroupEqualityComparer());



        var allLinks = groups.Select(x => x.Last())
        .GroupBy(x => x.tagNames != null)
        .ToDictionary(k => k.Key, v => v.AsEnumerable());

        var linkByState = allLinks.ContainsKey(false) ? allLinks[false] : new List<EventStateLinking>();
        var linkByTag = allLinks.ContainsKey(true) ? allLinks[true] : new List<EventStateLinking>();

        return new LinkRepo
        {
            LinksByState = linkByState.SelectMany(x => x.states.Select((s) =>
            {
                // multiple states can be connected to the same event.
                // multiply link and assigning one state each

                x.states = new List<BaseState> { s };
                return x;
            }))



            .GroupBy(x => x.states.First())
            .ToDictionary(k => k.Key, v => v.AsEnumerable()),

            LinksByTag = linkByTag.SelectMany(x => x.tagNames.Select((t) =>
            {
                // multiple states can be connected to the same event.
                // multiply link and assigning one state each

                x.tagNames = new List<string> { t };
                return x;
            }))
            .GroupBy(x => x.tagNames.First()).ToDictionary(k => k.Key, v => v.AsEnumerable())
        };
    }


    private IEnumerable<EventStateLinking> GetLinkersForTags(IEnumerable<string> tags, Dictionary<string, IEnumerable<EventStateLinking>> links)
    {
        var result = new List<EventStateLinking>();

        foreach (var tag in tags)
        {
            if (stateLinkers.LinksByTag.ContainsKey(tag))
            {
                result.AddRange(links[tag]);
            }
        }
        return result;
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
            links = (stateLinkers.LinksByState.ContainsKey(stateToSwitchTo) ?
            stateLinkers.LinksByState[stateToSwitchTo] : new List<EventStateLinking>())
            .Concat(GetLinkersForTags(stateToSwitchTo.tags, stateLinkers.LinksByTag)),

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