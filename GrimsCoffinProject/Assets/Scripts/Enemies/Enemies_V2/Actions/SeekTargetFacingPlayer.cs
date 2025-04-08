using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class SeekTargetFacingPlayer : EnemyAction
    {
        [Header("Physics")]
        public float nextWaypointDistance = 3f;
        public float targetRange = 1f;

        public float offsetMin;
        public float offsetMax;
        private float currentOffset;
        private float offsetTimer;
        private float maxOffsetTimer = 2;

        public float minDistanceRange;

        [Header("Animations")]
        public string animationWalkName;
        public string animationWalkBackName;
        public string idleAnimationTrigger;

        private bool shouldIdleAnim;
        private float idleAnimTimer;
        private float maxIdleAnimTimer = .1f;

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
        private bool foundPath;

        private bool isWalkingBack;

        private bool hasReachedEndOfPathOnce;
        private bool hasRequestedAttack;

        public override void OnStart()
        {
            //Get required components
            enemyCanvas = gameObject.GetComponentInChildren<Canvas>();

            //Start new path and timers
            foundPath = false;
            UpdatePath();
            repeatingTimer = repeatingNum;

            //Is waiting for player to return
            isWaiting = false;
            reachedEndOfPath = false;
            isWalkingBack = false;

            //Set state 
            enemyScript.enemyStateList.IsSeeking = true;

            hasReachedEndOfPathOnce = false;
            hasRequestedAttack = false;

            currentOffset = Random.Range(offsetMin, offsetMax);
            offsetTimer = maxOffsetTimer;
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
                    animator.Play(idleAnimationTrigger);
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

            //Idle animation status
            if(Mathf.Abs(rb.velocity.x) < 0.1f && !shouldIdleAnim)
            {
                shouldIdleAnim = true;
                idleAnimTimer = maxIdleAnimTimer;
            }

            if (shouldIdleAnim)
            {
                if (Mathf.Abs(rb.velocity.x) > 0.1f)
                    shouldIdleAnim = false;
                else
                    idleAnimTimer -= Time.deltaTime;
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(idleAnimationTrigger) && shouldIdleAnim && idleAnimTimer < 0)
                animator.Play(idleAnimationTrigger);
        }

        public override TaskStatus OnUpdate()
        {
            if (pathDistance > enemyScript.visionRange && foundPath)
                return TaskStatus.Failure;

            //Request the attack from the combat coordinator
            if (hasReachedEndOfPathOnce && !hasRequestedAttack)
            {
                enemyScript.CombatCoordinator.RequestAttack(enemyScript);
                hasRequestedAttack = true;
            }
            //Makes sure it doesn't get stuck thinking it has requested attack when it hasn't
            else if (hasRequestedAttack && !enemyScript.CombatCoordinator.IsWaitingForAttack(enemyScript))
                hasRequestedAttack = false;

            repeatingTimer -= Time.deltaTime;
            if (repeatingTimer < 0)// && !CheckEdge())
            {
                Vector2 targetLocation = GetTargetLocation();
                if (Mathf.Abs((targetLocation - rb.position).x) > .75f)
                    UpdatePath(targetLocation);
                else if(enemyScript.FindPlayerDistanceX() < minDistanceRange)
                    UpdatePath(targetLocation);


                //UpdatePath();
                repeatingTimer = repeatingNum;                
            }

            offsetTimer -= Time.deltaTime;
            if(offsetTimer < 0)
            {
                currentOffset = Random.Range(offsetMin, offsetMax);
                offsetTimer = maxOffsetTimer;
            }

            if (enemyScript.HasAttackTicket)
            {
                enemyScript.enemyStateList.IsAttacking = true;
            }

            return enemyScript.HasAttackTicket ? TaskStatus.Success : TaskStatus.Running;
        }

        public override void OnEnd()
        {
            if (pathDistance > enemyScript.visionRange)
                enemyScript.enemyStateList.IsSeeking = false;
        }

        private void PathFollow()
        {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                if(!hasReachedEndOfPathOnce)
                    hasReachedEndOfPathOnce = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            //Make sure direction is in x only
            //Vector2 direction = enemyScript.FindPlayerDirection();
            Vector2 direction = FindTargetDirection();

            if (direction.x != 0)
            {
                if (direction.x > 0)
                    direction.x = 1;
                else
                    direction.x = -1;
            }
            direction.y = 0;

            float targetSpeed = direction.x * enemyScript.seekSpeed;
            if (isWalkingBack)
                targetSpeed = targetSpeed * .95f;

            //Smooth changes to direction and speed using a lerp function
            float targetXRange = FindTargetDistance().x;
            float targetRatio = Mathf.Abs(targetXRange / (targetRange));
            float lerpValue = Mathf.Clamp(targetRatio, 0.1f, 1);

            //Debug.Log("Rb velocity: " + rb.velocity.x + " target speed: " + targetSpeed + " lerp value: " + lerpValue);
            targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpValue);

            if (isWalkingBack && CheckBackEdge())
                targetSpeed = 0;

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyScript.movementAccelAmount : enemyScript.movementDeaccelAmount;

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

            if (enemyScript.FindPlayerDistanceX() <= targetRange)
            {
                reachedEndOfPath = true;
                return;
            }

            Debug.Log(targetSpeed);

            //Start movement animation
            UpdateDirection();
            UpdateWalkDirection();

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationWalkName) && !isWalkingBack && !CheckBackEdge())
                animator.Play(animationWalkName);
            else if(!animator.GetCurrentAnimatorStateInfo(0).IsName(animationWalkBackName) && isWalkingBack && !CheckBackEdge())
                animator.Play(animationWalkBackName);
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                path = p;
                currentWaypoint = 0;

                pathDistance = path.GetTotalLength();
                foundPath = true;
            }
        }

        private void UpdatePath()
        {
            if (seeker.IsDone())
            {
                //Debug.Log("Updating path normally");

                Vector2 targetLocation = GetTargetLocation();

                float distance = Mathf.Pow((player.transform.position.x - rb.transform.position.x), 2)
                                    + Mathf.Pow((player.transform.position.y - rb.transform.position.y), 2);

                seeker.StartPath(rb.position, targetLocation, OnPathComplete);

                //Debug.Log("Path length: " + path.GetTotalLength());
            }
        }

        private void UpdatePath(Vector2 targetLocation)
        {
            if (seeker.IsDone())
            {
                //Debug.Log("Updating path with inputted target location");
                float distance = Mathf.Pow((player.transform.position.x - rb.transform.position.x), 2)
                                    + Mathf.Pow((player.transform.position.y - rb.transform.position.y), 2);

                seeker.StartPath(rb.position, targetLocation, OnPathComplete);
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
                    animator.Play(animationWalkName);
                    //animator.SetTrigger(animationTriggerName);
                }
                return false;
            }
        }

        private bool CheckBackEdge()
        {
            return (!enemyScript.backAirChecker.IsColliding || enemyScript.backWallChecker.IsColliding);
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

        private void UpdateWalkDirection()
        {
            //Find direction and update it
            Vector2 direction = enemyScript.FindPlayerDirection();

            if (enemyScript.enemyStateList.IsFacingRight)
            {
                if (FindTargetDirection().x > 0)
                {
                    isWalkingBack = false;
                }
                else
                {
                    isWalkingBack = true;
                }
            }
            else if (!enemyScript.enemyStateList.IsFacingRight)
            {
                if (FindTargetDirection().x < 0)
                {
                    isWalkingBack = false;
                }
                else
                {
                    isWalkingBack = true;
                }
            }
        }

        private Vector2 FindTargetDirection()
        {
            return FindTargetDistance().normalized;
        }

        private Vector2 FindTargetDistance()
        {
            return GetTargetLocation() - new Vector2(transform.position.x, transform.position.y); ;
        }

        private Vector2 GetTargetLocation()
        {
            //return new Vector2(player.transform.position.x + (-enemyScript.GetPlayerXDirection() * Random.Range(offsetMin, offsetMax + 1)), rb.position.y);
            return new Vector2(player.transform.position.x + (-enemyScript.GetPlayerXDirection() * currentOffset), rb.position.y);
        }
    }
}
