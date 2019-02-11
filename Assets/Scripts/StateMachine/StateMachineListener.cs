using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum StateMachineSubscriptionType
{
    LEAVE,
    ENTER,
    UPDATE
}

[System.Serializable]
public class StateMachineEventWithActiveLinking : UnityEvent<ActiveLinking> { }


[System.Serializable]
public class StateMachineSubscription
{

    public BaseState state;

    [System.NonSerialized]
    public StateMachine stateMachine;

    public StateMachineEventWithActiveLinking onStateEnter;
    public UnityEvent onStateLeave;
    public StateMachineEventWithActiveLinking onStateUpdate;

}

public class StateMachineListener : MonoBehaviour
{
    public StateMachine stateMachine;
    public List<StateMachineSubscription> subscriptions;

    // Use this for initialization
    void Start()
    {
        foreach (var sub in subscriptions)
        {

            sub.stateMachine = stateMachine;
            sub.stateMachine.AddSubscription(sub);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
