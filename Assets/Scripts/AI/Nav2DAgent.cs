using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEditor;
using UnityEngine.AI;

namespace Scripts.AI
{
    public enum NavAgentMode
    {
        PATH_FINDING,
        DIRECT
    }


    public class Nav2DAgent : MonoBehaviour
    {


        public float smoothAngleTime;

        internal void endMovement()
        {
            agent.isStopped = true;
        }

        public float neighborhoodRadius;
        public float mass;
        public float maxSteeringForce;
        public float maxSteeringSpeed;
        public string[] separationLayers = new String[] { "AI", "Player" };
        private HashSet<GameObject> ignoredSeparationNeighbors = new HashSet<GameObject>();

        private NavMeshAgent agent;



        [System.NonSerialized]
        public bool separateFromPlayer = true;

        public bool NeightborAvoidance
        {
            set
            {
                this.neightborAvoidance = value;
            }
        }

        public float Speed
        {
            set
            {
                this.speed = value;
            }
        }



        public bool isMoving
        {
            get
            {
                return agent.pathPending;
            }
        }


        private bool _moving;

        private float targetsRadius;
        private bool neightborAvoidance = true;
        private float speed;


        private NavAgentMode mode;
        private ConcurrentStack<Vector3> path;
        private Vector3 currentTarget;
        private float startTime;
        private Vector3 startPos;
        private Rigidbody2D rb;
        private float yVelocity = 0.0f;

        [System.NonSerialized]
        public Vector2 velocity = Vector2.zero;

        private Character character;



        public void setTarget(
            Vector3 worldPos,
            NavAgentMode mode,
            float targetsRadius,
            float speed,
            bool neightborAvoidance = true
        )
        {

            this.targetsRadius = targetsRadius;
            this.neightborAvoidance = neightborAvoidance;
            this.speed = speed;


            //Debug.Log("Requested path");

            agent.destination = worldPos;
            this.agent.isStopped = false;
            agent.speed = speed;

            this._moving = true;



        }





        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            path = new ConcurrentStack<Vector3>();

            character = GetComponent<Character>();

            agent = GetComponent<NavMeshAgent>();


        }

        // Update is called once per frame
        void Update()
        {


            // Debug.Log(ignoredSeparationNeighbors.Count);


            // // if (this.path != null)
            // // {
            // //     for (var i = 1; i < this.path.Count; i++)
            // //     {

            // //         Debug.DrawLine(this.path.ElementAt(i - 1), this.path.ElementAt(i), Color.red);
            // //     }
            // // }

            // Vector2 desiredVelocity = ((_moving ? (currentTarget - transform.position).normalized : Vector3.zero) +
            //  computeSeparation()) * Time.fixedDeltaTime * speed;

            // var sterring = Vector2.ClampMagnitude((desiredVelocity - velocity), maxSteeringForce) / mass;

            // velocity = Vector2.ClampMagnitude(velocity + sterring, maxSteeringSpeed);




            // // set if character is moving for animations
            // character.moving = _moving;

        }

        void FixedUpdate()
        {
            //rb.MovePosition(rb.position + velocity);s
        }


        public void addSeparationIgnored(GameObject obj)
        {
            if (!ignoredSeparationNeighbors.Contains(obj))
                ignoredSeparationNeighbors.Add(obj);
        }

        public void removeSeparationIgnored(GameObject obj)
        {
            ignoredSeparationNeighbors.Remove(obj);
        }

        private Vector3 computeSeparation()
        {
            var resultVec = Vector3.zero;

            if (neightborAvoidance)
            {
                var neighbors = Physics2D.OverlapCircleAll(
                    transform.position,
                    neighborhoodRadius,
                    LayerMask.GetMask(separationLayers)
                );

                foreach (var n in neighbors)
                {
                    if (!ignoredSeparationNeighbors.Contains(n.gameObject))
                        resultVec += n.transform.position - transform.position;
                }

                resultVec *= -1;
            }

            return resultVec.normalized;
        }
    }
}
