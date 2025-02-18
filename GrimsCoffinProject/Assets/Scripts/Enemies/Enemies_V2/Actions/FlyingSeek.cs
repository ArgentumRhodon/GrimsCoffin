using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class FlyingSeek : EnemyAction
    {
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

        private Canvas enemyCanvas;

        private Collider2D visionRange;


        public override void OnStart()
        {
            //Get required components
            enemyCanvas = gameObject.GetComponentInChildren<Canvas>();

            //Start new path and timers
            UpdatePath();
            repeatingTimer = repeatingNum;

            //Is waiting for player to return
            reachedEndOfPath = false;

            //Set vision 
            visionRange = enemyScript.visionCollider;
            enemyScript.enemyStateList.IsSeeking = true;
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

            if (!IsOverlapping())
                return TaskStatus.Failure;

            return reachedEndOfPath ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            enemyScript.enemyStateList.IsSeeking = false;
        }

        private void PathFollow()
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

            //Make sure direction is in x only
            //Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 direction = enemyScript.FindPlayerDirection();
            Vector2 targetSpeed = direction * enemyScript.seekSpeed;

            //Smooth changes to direction and speed using a lerp function
            targetSpeed = Vector2.Lerp(rb.velocity, targetSpeed, 1);

            float accelRate = (Mathf.Abs(targetSpeed.x) > 0.01f) ? enemyScript.movementAccelAmount : enemyScript.movementDeccelAmount;
            float accelRateY = (Mathf.Abs(targetSpeed.y) > 0.01f) ? enemyScript.movementAccelAmount : enemyScript.movementDeccelAmount;

            //Calculate difference between current velocity and desired velocity
            Vector2 speedDif = targetSpeed - rb.velocity;
            //Calculate force along x-axis to apply to thr player
            Vector2 movement = new Vector2(speedDif.x * accelRate, speedDif.y * accelRateY);


            //Debug.Log("Ghost movement: " +  movement);
            rb.AddForce(movement, ForceMode2D.Force);


            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (movement.x >= 0.01f)
            {
                enemyScript.FaceRight(true);
            }
            else if (movement.x <= -0.01f)
            {
                enemyScript.FaceRight(false);
            }

            /*            if (rb.velocity.x >= 0.01f)
                        {
                            enemyScript.FaceRight(true);
                        }
                        else if (rb.velocity.x <= -0.01f)
                        {
                            enemyScript.FaceRight(false);
                        }*/

            if (enemyScript.FindPlayerDistanceX() <= targetRange && enemyScript.FindPlayerDistanceY() <= 1)
            {
                reachedEndOfPath = true;
                return;
            }

            //Start movement animation
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationTriggerName))
                animator.Play(animationTriggerName);
            //animator.SetTrigger(animationTriggerName);
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;
            }
        }

        private void UpdatePath()
        {
            if (seeker.IsDone())
            {
                float distance = Mathf.Pow((player.transform.position.x - rb.transform.position.x), 2)
                                    + Mathf.Pow((player.transform.position.y - rb.transform.position.y), 2);

                seeker.StartPath(rb.position, player.transform.position, OnPathComplete);

                //Debug.Log("Path length: " + path.GetTotalLength());
            }
        }

        private void UpdateDirection()
        {
            //Find direction and update it
            Vector2 direction = enemyScript.FindPlayerDirection();

            if (direction.x > 0 && !enemyScript.enemyStateList.IsFacingRight)
            {
                enemyScript.FaceRight(true);
            }
            else if (direction.x < 0 && enemyScript.enemyStateList.IsFacingRight)
            {
                enemyScript.FaceRight(false);
            }
        }

        private bool IsOverlapping()
        {
            //Check for colliders overlapping
            Collider2D[] collidersToCheck = new Collider2D[10];

            //Debug.Log("Colliders to Check Size" + collidersToCheck.Length);

            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;

            int colliderCount = Physics2D.OverlapCollider(visionRange, filter, collidersToCheck);
            //Debug.Log("Colliders Count" + colliderCount);


            //Go through all colliders and check to see if it is the player
            for (int i = 0; i < colliderCount; i++)
            {
                if (collidersToCheck[i].gameObject.tag == "Player")
                    return true;
            }
            return false;
        }


    }
}
