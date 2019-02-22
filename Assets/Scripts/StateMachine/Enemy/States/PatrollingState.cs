

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Scripts.NPC;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "PatrollingState", menuName = "StateMachine/States/Enemy/PatrollingState")]
public class PatrollingState : BaseState
{
    public float findClosestBuildingRadius = 30;
    public string[] buildingFindLayers = new String[] { "Nav2DObstacle" };

    public PatrollingState()
    {
        this.stateName = "patrolling";
    }
    override public void Leave(StateMachine stateMachine)
    {

    }
    override public void Enter(StateMachine stateMachine, EventMessage eventResponse, ActiveLinking activeLinking)
    {
        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();

        npc.VisionAngleModifier = 1f;
        npc.VisionRadiusModifier = 1f;

        activeLinking.linkingProperties.Add("corners", new Queue<Vector2>());
        activeLinking.linkingProperties.Add("buildings", new Queue<Queue<Vector2>>());

    }
    override public void StateUpdate(StateMachine stateMachine, ActiveLinking activeLinking)
    {

        NonPlayerCharacter npc = stateMachine.GetComponent<NonPlayerCharacter>();
        Character character = stateMachine.GetComponent<Character>();

        character.currentSpeed = CharacterSpeed.SLOW;


        if (!npc.nav2DAgent.isMoving())
        {
            Debug.Log("moving is false");

            var colliders = Physics2D.OverlapCircleAll(stateMachine.transform.position,
             findClosestBuildingRadius, LayerMask.GetMask(buildingFindLayers));

            var buildingQueue = activeLinking.GetValueOrDefault<Queue<Queue<Vector2>>>("buildings");
            var cornerQueue = activeLinking.GetValueOrDefault<Queue<Vector2>>("corners");

            if (cornerQueue.Count < 1)
            {


                if (buildingQueue.Count < 1)
                {

                    var closestColliders = colliders.OrderBy(
                        x => (x.transform.position - stateMachine.transform.position).magnitude
                    );

                    foreach (var collider in closestColliders)
                    {

                        var c = collider.GetComponentInParent<BuildingController>()?.WorldSpaceCorners;

                        if (c == null)
                            continue;

                        buildingQueue.Enqueue(new Queue<Vector2>(c));

                    }

                }


                var corners = new List<Vector2>(buildingQueue.Dequeue());


                // patrolling around building can be clockwise or anticlockwise
                if (Random.value > 0.5)
                    corners.Reverse();


                var closesCornerIndex = corners.FindIndex(x =>
                    x == corners.MinBy(c => (c - (Vector2)stateMachine.transform.position).magnitude).First()
                );


                // shift the values so that the closest corner is first
                for (var i = 0; i < corners.Count; i++)
                {
                    cornerQueue.Enqueue(corners[(closesCornerIndex + i) % corners.Count]);
                }


            }
            else
            {
                npc.MoveToOrFollow(
                    cornerQueue.Dequeue()
                );
            }
        }


    }
}



