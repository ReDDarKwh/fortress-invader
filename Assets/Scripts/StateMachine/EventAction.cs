using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public enum EventActionType
{
    TRANSITION,
    NONE,
    REMOVE,
    ADD,
    ADD_OVERRIDE,
    TRANSITION_OVERRIDE
}


[System.Serializable]
public class EventAction
{
    public EventActionType actionType = EventActionType.TRANSITION;

    [System.NonSerialized]
    public BaseState transitionToState;
    [System.NonSerialized]
    public List<BaseState> addStates;

    [System.NonSerialized]
    public BaseState fromState;
}
