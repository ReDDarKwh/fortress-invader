using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Scripts.AI
{
    public enum NavAgentMode
    {
        PATH_FINDING,
        DIRECT
    }


    public class Nav2DAgent : MonoBehaviour
    {

        public Nav2D navGrid;
        public float smoothAngleTime;
        public float neighborhoodRadius;
        public float mass;
        public float maxSteeringForce;
        public float maxSteeringSpeed;
        public string[] separationLayers = new String[] { "AI", "Player" };
        private HashSet<GameObject> ignoredSeparationNeighbors = new HashSet<GameObject>();



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
        private Vector2 velocity = Vector2.zero;
        private bool moving = false;



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

            switch (mode)
            {
                case NavAgentMode.PATH_FINDING:
                    navGrid.RequestPath(transform.position, worldPos, PathCallback);
                    break;

                case NavAgentMode.DIRECT:
                    this.currentTarget = worldPos;

                    break;
            }

            this.moving = true;
        }

        public void PathCallback(IEnumerable<Nav2dNode> path)
        {
            if (path == null)
            {
                this.moving = false;
                return;
            }

            this.path.Clear();

            foreach (var pos in path.Select(x => x.worldPos))
            {
                this.path.Push(pos);
            }

            // skip the first node no jitter on path change.
            setNextTarget();
            setNextTarget();
        }

        public bool isMoving()
        {
            return this.moving;
        }

        public void endMovement()
        {
            this.moving = false;
        }

        private void setNextTarget()
        {
            if (path != null && path.Count > 0)
            {
                path.TryPop(out currentTarget);
                return;
            }
            this.moving = false;
        }

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            navGrid = GameObject.FindGameObjectWithTag("MainNavGrid").GetComponent<Nav2D>();
            path = new ConcurrentStack<Vector3>();
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log("update agent");
            Vector2 desiredVelocity = ((isMoving() ? (currentTarget - transform.position).normalized : Vector3.zero) +
             computeSeparation()) * Time.deltaTime * speed;

            var sterring = Vector2.ClampMagnitude((desiredVelocity - velocity), maxSteeringForce) / mass;

            velocity = Vector2.ClampMagnitude(velocity + sterring, maxSteeringSpeed);

            rb.MovePosition(rb.position + velocity);

            if (isMoving())
            {
                var angle = Mathf.SmoothDampAngle(
                    rb.transform.rotation.eulerAngles.z,
                    Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg,
                    ref yVelocity,
                    smoothAngleTime
                );
                rb.transform.rotation = Quaternion.Euler(0, 0, angle);

                if ((transform.position - currentTarget).magnitude < targetsRadius)
                {
                    setNextTarget();
                }
            }
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
