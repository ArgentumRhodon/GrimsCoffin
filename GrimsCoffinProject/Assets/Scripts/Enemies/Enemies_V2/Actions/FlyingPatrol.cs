using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;

namespace Core.AI
{
    public class FlyingPatrol : EnemyAction
    {
        public float wanderRange = 3f;

        private Vector3 target1;
        private Vector3 target2;
        private Vector3 currentTarget;

        private bool patrolTarget;

        [Header("Physics")]
        public float speed = 50f;
        public float nextWaypointDistance = 3f;
        public float targetRange = 1f;

        [Header("Animations")]
        public string animationTriggerName;
        public string nextAnimationTrigger;

        //Pathfinding tools
        private Path path;
        private int currentWaypoint = 0;
        private bool reachedEndOfPath = false;

        //Update path modifiers
        private float repeatingNum = .2f;
        private float repeatingTimer;

        // Start is called before the first frame update
        public override void OnStart()
        {           
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationTriggerName))
                animator.Play(animationTriggerName);

            //Target patrol areas
            target1 = this.transform.position + new Vector3(0, wanderRange, 0);
            target2 = this.transform.position + new Vector3(0, -wanderRange, 0);

            currentTarget = target1;
        }

        public override void OnFixedUpdate()
        {
            PathFollow();
        }

        public override TaskStatus OnUpdate()
        {
            repeatingTimer -= Time.deltaTime;
            if (repeatingTimer < 0)// && !CheckEdge())
            {
                UpdatePath();
                repeatingTimer = repeatingNum;
            }

            return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            enemyScript.enemyStateList.IsSeeking = false;
        }

        void PathFollow()
        {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 targetPos = new Vector2(currentTarget.x, currentTarget.y);
            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);

            //Make sure direction is in x only
            Vector2 direction = (targetPos - enemyPos).normalized;

            //Check to see if floating or walking
            direction.x = 0;

            Vector2 force = new Vector2();// = direction * speed * Time.deltaTime;

            if (targetPos.y > enemyPos.y)
            {
                force.y = 1 * speed;
            }
            else
            {
                force.y = -1 * speed;
            }
            
            rb.AddForce(force);

            if (currentWaypoint >= path.vectorPath.Count)
            {
                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
            }
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;

                float distance = Vector2.Distance(rb.position, currentTarget);
                if (patrolTarget && distance < 0.5f)
                {
                    patrolTarget = false;
                }
                else if (distance < 0.5f)
                {
                    patrolTarget = true;
                }
                
            }
        }

        private void UpdatePath()
        {
            if (seeker.IsDone())
            {
                //If statement to decide which target to go to
                if (patrolTarget)
                {
                    currentWaypoint = 0;
                    seeker.StartPath(rb.position, target1, OnPathComplete);
                    currentTarget = target1;
                }
                else
                {
                    currentWaypoint = 0;
                    seeker.StartPath(rb.position, target2, OnPathComplete);
                    currentTarget = target2;
                }              
            }
        }


    }
}
