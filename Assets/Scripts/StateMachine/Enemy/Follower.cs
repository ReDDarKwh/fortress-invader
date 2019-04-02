using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AI;
using Scripts.NPC;

public class Follower : MonoBehaviour
{
    private Nav2DAgent navAgent;
    private NonPlayerCharacter nonPlayerCharacter;
    private StateMachine stateMachine;
    private Character character;
    [System.NonSerialized]
    public Leader leader;
    [System.NonSerialized]
    public Vector3 offSetFromLeader;

    public BaseEvent followDoneEvent;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<Nav2DAgent>();
        nonPlayerCharacter = GetComponent<NonPlayerCharacter>();
        stateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {

        if (leader.enabled)
        {
            var offsetVec = transform.position + (leader.character.moveDirection * offSetFromLeader);
            Debug.DrawLine(transform.position, offsetVec, Color.red);
            nonPlayerCharacter.MoveToOrFollow(offsetVec);
        }
        else
        {
            // trigger leave following group if leader disabled
            stateMachine.TriggerEvent(followDoneEvent, EventMessage.EmptyMessage);
        }

    }
}
