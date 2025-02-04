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
        private Seeker seeker;

        //Update path modifiers
        private float repeatingNum = .2f;
        private float repeatingTimer;

        private bool isWaiting;
        private Canvas enemyCanvas;

        private Collider2D visionRange; 


        public override void OnStart()
        {
            //Get required components
            seeker = GetComponent<Seeker>();
            enemyCanvas = gameObject.GetComponentInChildren<Canvas>();

            //Start new path and timers
            UpdatePath();
            repeatingTimer = repeatingNum;

            //Is waiting for player to return
            isWaiting = false;
            reachedEndOfPath = false;

            //Set vision 
            visionRange = enemyScript.visionCollider;
        }

        public override TaskStatus OnUpdate()
        {
            repeatingTimer -= Time.deltaTime;
            if (repeatingTimer < 0)// && !CheckEdge())
            {
                UpdatePath();
                repeatingTimer = repeatingNum;
            }

            if(!CheckEdge())
                PathFollow();
            else if(CheckEdge())
            {
                if (!isWaiting)
                {
                    rb.velocity = Vector2.zero;
                    isWaiting = true;
                    animator.SetTrigger(nextAnimationTrigger);
                }
                else if(isWaiting)
                {
                    //Debug.Log("Is waiting");
                    UpdateDirection();
                    if (FindPlayerDistance() <= targetRange)
                    {
                        reachedEndOfPath = true;
                    }
                }
            }

            if(!IsOverlapping())
                return TaskStatus.Failure;

            return reachedEndOfPath ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            animator.SetTrigger(nextAnimationTrigger);
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
            Vector2 direction = FindPlayerDirection();

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
                enemyScript.FaceRight();
            }
            else if (movement <= -0.01f)
            {
                enemyScript.FaceRight(false);
            }

            if (FindPlayerDistance() <= targetRange)
            {
                reachedEndOfPath = true;
                return;
            }

            //Start movement animation
            animator.SetTrigger(animationTriggerName);
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
                    animator.SetTrigger(animationTriggerName);
                }
                return false;
            }
        }

        private void UpdateDirection()
        {
            //Find direction and update it
            Vector2 direction = FindPlayerDirection();

            if(direction.x > 0 && !enemyScript.IsFacingRight)
            {
                enemyScript.FaceRight();
            }
            else if(direction.x < 0 && enemyScript.IsFacingRight)
            {
                enemyScript.FaceRight(false);
            }
        }

        private bool IsOverlapping()
        {
            //Check for colliders overlapping
            Collider2D[] collidersToCheck = new Collider2D[10];
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            int colliderCount = Physics2D.OverlapCollider(visionRange, filter, collidersToCheck);

            //Go through all colliders and check to see if it is the player
            for (int i = 0; i < colliderCount; i++)
            {
                if (collidersToCheck[i].gameObject.tag == "Player")
                    return true;
            }
            return false;
        }
    
        private float FindPlayerDistance()
        {
            return Mathf.Abs(player.transform.position.x - transform.position.x);
        }

        private Vector2 FindPlayerDirection()
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);

            return (playerPos - enemyPos).normalized;
        }
    }
}
