using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AI;
using System.Linq;
using MoreLinq;

namespace Scripts.NPC
{
    [RequireComponent(typeof(Nav2DAgent))]
    [RequireComponent(typeof(Character))]
    public class NonPlayerCharacter : MonoBehaviour
    {

        public float VisionAngle
        {
            get
            {
                return baseVisionAngle * VisionAngleModifier;
            }
        }

        public float VisionRadius
        {
            get
            {
                return baseVisionRadius * VisionRadiusModifier;
            }
        }

        public float VisionRadiusModifier;
        public float VisionAngleModifier;

        [SerializeField]
        private float baseVisionAngle;

        [SerializeField]
        private float baseVisionRadius;

        public float earshotRadius;

        public float pathToTargetUpdateDistance;
        public float pathFindingTargetRadius;
        public string[] TargetLayers;
        public string[] ViewBlockingLayers;

        // state machine attributes

        internal Vector3 searchingPosition;
        internal Character characterTarget;
        internal Vector3 oldPositionTarget;

        // ---

        internal Nav2DAgent nav2DAgent;
        internal Character character;

        void Start()
        {
            nav2DAgent = GetComponent<Nav2DAgent>();
            character = GetComponent<Character>();
            nav2DAgent.Speed = character.GetSpeed();

            nav2DAgent.addSeparationIgnored(GameObject.FindWithTag("Player"));
        }

        public void MoveToOrFollow(Vector3 target)
        {
            nav2DAgent.setTarget(
                target,
                NavAgentMode.PATH_FINDING,
                pathFindingTargetRadius,
                character.GetSpeed()
            );
        }

        public void StopMoveToOrFollow()
        {
            nav2DAgent.endMovement();
        }

        public GameObject TargetInView(GameObject target)
        {

            if (target == null)
            {
                return null;
            }

            var EnemyToTargetVec = target.transform.position - transform.position;

            if (EnemyToTargetVec.magnitude > VisionRadius)
            {
                return null;
            }

            Debug.DrawRay(transform.position, EnemyToTargetVec);

            // if is close enough to be heard.
            if (EnemyToTargetVec.magnitude < earshotRadius)
            {
                return target;
            }

            // if is in view cone
            if (!(Quaternion.Angle(
                  Quaternion.LookRotation(transform.right, Vector3.forward),
                  Quaternion.LookRotation(
                     EnemyToTargetVec.normalized, Vector3.forward)
                    ) < VisionAngle / 2
                )
                )
                return null;

            var hit = Physics2D.Raycast(
                transform.position,
                EnemyToTargetVec,
                EnemyToTargetVec.magnitude,
                LayerMask.GetMask(ViewBlockingLayers)
            );

            return hit.collider == null ? target : null;
        }

        public IList<Collider2D> getTargetsInViewRange()
        {
            return Physics2D.OverlapCircleAll(
                transform.position,
                VisionRadius,
                LayerMask.GetMask(TargetLayers)
            );
        }

        private Collider2D getClosestTargetInViewRange()
        {
            var targets = getTargetsInViewRange();
            return targets.MinBy(
                x =>
                (transform.position - x.transform.position).magnitude
            ).FirstOrDefault();
        }

        public GameObject ClosestTargetInView()
        {
            return TargetInView(getClosestTargetInViewRange()?.gameObject);
        }

    }
}

