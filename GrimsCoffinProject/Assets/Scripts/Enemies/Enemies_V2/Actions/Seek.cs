using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Seek : EnemyAction
    {
        [Header("Physics")]
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

        private bool isWaiting;
        private Canvas enemyCanvas;

        private Collider2D visionCollider;

        private float pathDistance;

        public override void OnStart()
        {
            //Get required components
            enemyCanvas = gameObject.GetComponentInChildren<Canvas>();

            //Start new path and timers
            UpdatePath();
            repeatingTimer = repeatingNum;

            //Is waiting for player to return
            isWaiting = false;
            reachedEndOfPath = false;

            //Set vision 
            visionCollider = enemyScript.visionCollider;
            enemyScript.enemyStateList.IsSeeking = true;
        }

        public override void OnFixedUpdate()
        {
            if (!CheckEdge())
                PathFollow();
            else if (CheckEdge())
            {
                if (!isWaiting)
                {
                    rb.velocity = Vector2.zero;
                    isWaiting = true;
                    animator.Play(animationTriggerName);                 
                }
                else if (isWaiting)
                {
                    UpdateDirection();
                    if (enemyScript.FindPlayerDistanceX() <= targetRange)
                    {
                        reachedEndOfPath = true;
                    }
                }
            }
        }

        public override TaskStatus OnUpdate()
        {       
            if (pathDistance > enemyScript.visionRange)
                return TaskStatus.Failure;

                repeatingTimer -= Time.deltaTime;
            if (repeatingTimer < 0)// && !CheckEdge())
            {
                UpdatePath();
                repeatingTimer = repeatingNum;
            }

            /*if(!IsOverlapping())
                return TaskStatus.Failure;*/

            return reachedEndOfPath ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (pathDistance > enemyScript.visionRange)
                enemyScript.enemyStateList.IsSeeking = false;

            if(reachedEndOfPath)
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
            Vector2 direction = enemyScript.FindPlayerDirection();

            if (direction.x != 0)
            {
                if (direction.x > 0)
                    direction.x = 1;
                else
                    direction.x = -1;
            }
            direction.y = 0;

            float targetSpeed = direction.x * enemyScript.seekSpeed;

            //Smooth changes to direction and speed using a lerp function
            targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 1);

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyScript.movementAccelAmount : enemyScript.movementDeccelAmount;

            //Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - rb.velocity.x;
            //Calculate force along x-axis to apply to thr player
            float movement = speedDif * accelRate;

            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);


            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (movement >= 0.01f)
            {
                enemyScript.FaceRight(true);
            }
            else if (movement <= -0.01f)
            {
                enemyScript.FaceRight(false);
            }

            if (enemyScript.FindPlayerDistanceX() <= targetRange)
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

                pathDistance = path.GetTotalLength();
                Debug.Log(pathDistance);
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

        private bool CheckEdge()
        {
            //Check to see if it is not colliding with the ground or is colliding with a wall
            if (!enemyScript.airChecker.IsColliding || enemyScript.wallChecker.IsColliding)
            {
                return true;
            }
            else
            {
                if (isWaiting)
                {
                    isWaiting = false;
                    animator.Play(nextAnimationTrigger);
                    //animator.SetTrigger(animationTriggerName);
                }
                return false;
            }
        }

        private void UpdateDirection()
        {
            //Find direction and update it
            Vector2 direction = enemyScript.FindPlayerDirection();

            if(direction.x > 0 && !enemyScript.enemyStateList.IsFacingRight)
            {
                enemyScript.FaceRight(true);
            }
            else if(direction.x < 0 && enemyScript.enemyStateList.IsFacingRight)
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

            int colliderCount = Physics2D.OverlapCollider(visionCollider, filter, collidersToCheck);
            Debug.Log("Colliders Count" + colliderCount);


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
