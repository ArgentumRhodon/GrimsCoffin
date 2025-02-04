using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCharacterController;
using Unity.VisualScripting;

namespace Core.AI
{
    public class Seek : EnemyAction
    {
        [Header("Physics")]
        public float speed = 200f; //, jumpForce = 100f;
        public float nextWaypointDistance = 3f;

        //Jump Variables
        /*        public float jumpNodeHeightRequirement = 0.8f;
                public float jumpModifier = 0.3f;
                public float jumpCheckOffset = 0.1f;*/
        //private bool isOnCoolDown;

        /*        [Header("Custom Behavior")]
                public bool jumpEnabled = false, isJumping, isInAir;
                public bool directionLookEnabled = true;*/

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
        //private RaycastHit2D isGrounded;


        public override void OnStart()
        {
            seeker = GetComponent<Seeker>();
            enemyCanvas = gameObject.GetComponentInChildren<Canvas>();
            UpdatePath();
            repeatingTimer = repeatingNum;
            isWaiting = false;
            visionRange = enemyScript.visionCollider;
        }

        public override TaskStatus OnUpdate()
        {
            repeatingTimer -= Time.deltaTime;
            if (repeatingTimer < 0 && !CheckEdge())
            {
                UpdatePath();
                repeatingTimer = repeatingNum;
            }

            if(!CheckEdge())
                PathFollow();
            else if(CheckEdge())
            {
                Debug.Log("At edge");
                if(!isWaiting)
                {
                    rb.velocity = Vector2.zero;
                    isWaiting = true;
                    animator.SetTrigger("Idle");
                }
                else if(isWaiting)
                {
                    UpdateDirection();
                }
            }

            if(!IsOverlapping())
                return TaskStatus.Failure;

            return reachedEndOfPath ? TaskStatus.Success : TaskStatus.Running;
        }

        private void PathFollow()
        {
            if (path == null)
                return;

            Debug.Log("Following Path");

/*            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }*/
               
            if (currentWaypoint >= path.vectorPath.Count)
            {
                if (player.transform.position == transform.position)
                {
                    reachedEndOfPath = true;
                    return;
                }
                else
                {
                    reachedEndOfPath = false;
                }             
            }
            else
            {
                reachedEndOfPath = false;
            }


            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);

            //Make sure direction is in x only
            Vector2 direction = (playerPos - enemyPos).normalized;
            direction.y = 0;
            Vector2 force = new Vector2();// = direction * speed * Time.deltaTime;


            //Issues with y movement, so only moving the x position
            if (playerPos.x > enemyPos.x)
            {
                force.x = 1 * speed * Time.deltaTime;              
            }
            else
            {
                force.x = -1 * speed * Time.deltaTime;
            }

            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (force.x >= 0.01f)
            {
                rb.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (force.x <= -0.01f)
            {
                rb.transform.localScale = new Vector3(-1, 1, 1);
            }
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
                    animator.SetTrigger("Walk");
                }
                return false;
            }
        }

        private void UpdateDirection()
        {
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);

            Vector2 direction = (playerPos - enemyPos).normalized;

            if(direction.x > 0 && !enemyScript.IsFacingRight)
            {
                TurnAround();
            }
            else if(direction.x < 0 && enemyScript.IsFacingRight)
            {
                TurnAround();
            }
        }

        private void TurnAround()
        {
            //Transform local scale of object
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            enemyScript.IsFacingRight = !enemyScript.IsFacingRight;
            enemyScript.Direction *= -1;

            //Updates scale of UI so that it is always facing right
            Vector3 tempScale = enemyCanvas.transform.localScale;
            tempScale.x *= -1;
            enemyCanvas.transform.localScale = tempScale;
        }

        public bool IsOverlapping()
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
    }
}
