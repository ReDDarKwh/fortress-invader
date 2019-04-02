

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using Scripts.AI;
using Scripts.NPC;
using Scripts.Spells;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "PatrollingState", menuName = "StateMachine/States/Enemy/PatrollingState")]
public class PatrollingState : BaseState
{
    public float findClosestBuildingRadius = 100;
    public string[] buildingFindLayers = new String[] { "Nav2DObstacle" };
    public string[] spellTargetLayers = new String[] { "SpellTarget" };

    private Nav2D navGrid;



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


        Nav2DAgent navAgent = stateMachine.GetComponent<Nav2DAgent>();

        character.currentSpeed = CharacterSpeed.SLOW;


        if (!npc.nav2DAgent.isMoving && !npc.nav2DAgent.isWaitingForPath)
        {
            //Debug.Log("moving is false");

            var colliders = Physics2D.OverlapCircleAll(stateMachine.transform.position,
             findClosestBuildingRadius, LayerMask.GetMask(buildingFindLayers));


            if (colliders.Count() <= 0)
            {
                return;
            }

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

                        var c = collider.GetComponentInParent<BuildingController>();

                        // if has building component and parent building is on building layer (walls are building that cannot be patrolled)
                        if (c == null || c.gameObject.layer != LayerMask.NameToLayer("Building"))
                        {
                            var l = LayerMask.GetMask("Building");
                            continue;

                        }

                        buildingQueue.Enqueue(new Queue<Vector2>(c.WorldSpaceCorners));

                    }

                }


                if (buildingQueue.Count < 1)
                {

                    return;
                }


                var corners = new List<Vector2>(buildingQueue.Dequeue())
                .Where(x =>
                {

                    // only take corners that are not inside a active spell
                    var c = Physics2D.OverlapPointAll(x, LayerMask.GetMask(spellTargetLayers));
                    foreach (var collider in c)
                    {
                        var target = collider.GetComponent<MagicTargetBase>();
                        if (!target.spellDone)
                        {
                            return false;
                        }
                    }

                    // and the corner is accessable;
                    return navAgent.navGrid.CanBeReached(x);
                }).ToList();


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



