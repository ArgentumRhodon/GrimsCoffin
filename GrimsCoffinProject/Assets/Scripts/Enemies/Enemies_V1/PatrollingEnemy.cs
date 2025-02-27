using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : Enemy
{
    [Header("Targets")]
    [SerializeField] private float wanderRange = 3f;

    private Vector3 target1;
    private Vector3 target2;
    private Vector3 currentTarget;

    private bool patrolTarget;
    private Vector3 spawnLocation;


    [Header("Movement")]
    //[SerializeField] private float speed = 200f;
    [SerializeField] private float nextWaypointDistance = 3f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private float jumpModifier = 0.3f;
    [SerializeField] private float jumpCheckOffset = 0.1f;

    [Header("Behavior")]
    [SerializeField] private bool isIdle;
    [SerializeField] private bool jumpEnabled = true;
    [SerializeField] private bool isWalking;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    private bool reachedEndOfPath = false;

    protected override void Start()
    {
        base.Start();

        isIdle = true;

        spawnLocation = this.transform.position;
        if (isWalking)
        {
            target1 = this.transform.position + new Vector3(wanderRange, 0, 0);
            target2 = this.transform.position + new Vector3(-wanderRange, 0, 0);
        }
        else
        {
            target1 = this.transform.position + new Vector3(0, wanderRange, 0);
            target2 = this.transform.position + new Vector3(0, -wanderRange, 0);
        }
    

        currentTarget = target1;

        //Square range at start to make it easier to calculate later
        visionRange = visionRange * visionRange;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        //seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    protected override void FixedUpdate()
    {
        if (!enemyStateList.IsSleeping)
        {
            PathFollow();          
        }
        CheckCollisionWithPlayer();
    }

    //Path calculation methods
    void PathFollow()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (!isIdle)
            {
                //Debug.Log("This is running");
                reachedEndOfPath = true;
                return;
            }
            else
            {
                if (currentTarget == enemy.transform.position)
                {
                    reachedEndOfPath = true;
                    return;
                }
                else
                {
                    reachedEndOfPath = false;
                }
            }
        }
        else
        {
            reachedEndOfPath = false;
        }

        //Used as a check for jumping
        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);

        Vector2 targetPos = new Vector2(currentTarget.x, currentTarget.y);
        Vector2 enemyPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y);

        //Make sure direction is in x only
        Vector2 direction = (targetPos - enemyPos).normalized;

        //Check to see if floating or walking
        if (isWalking)
            direction.y = 0;
        else
            direction.x = 0;

        Vector2 force = new Vector2();// = direction * speed * Time.deltaTime;


        //Issues with y movement, so only moving the x position

        if (isWalking)
        {
            if (targetPos.x > enemyPos.x)
            {
                force.x = 1 * movementSpeed * Time.deltaTime;
            }
            else
            {
                force.x = -1 * movementSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (targetPos.y > enemyPos.y)
            {
                force.y = 1 * movementSpeed * Time.deltaTime;
            }
            else
            {
                force.y = -1 * movementSpeed * Time.deltaTime;
            }
        }

        rb.AddForce(force);

        if(currentWaypoint >= path.vectorPath.Count)
        {
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }

        if (force.x >= 0.01f)
        {
            //enemyGFX.localScale *= new Vector3(-1, 1, 1);

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            enemyGFX.transform.localScale = scale;
        }
        else if (force.x <= -0.01f)
        {
            //enemyGFX.localScale = new Vector3(1, 1, 1);

            Vector3 scale = transform.localScale;
            scale.x *= 1;
            enemyGFX.transform.localScale = scale;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            if (isIdle)
            {
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
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            currentWaypoint = 0;
            if (TargetInRange())
            {
                //Debug.Log("Tracking player");
                currentWaypoint = 0;
                seeker.StartPath(rb.position, playerTarget.position, OnPathComplete);
                currentTarget = playerTarget.transform.position;
            }
            //Move between the two idle targets
            else
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

    private bool TargetInRange()
    {
        float distance = Mathf.Pow((playerTarget.transform.position.x - enemy.transform.position.x), 2)
                                + Mathf.Pow((playerTarget.transform.position.y - enemy.transform.position.y), 2);

        bool inRange = distance < visionRange;

        if (inRange)
        {
            isIdle = false;
        }
        else
        {
            isIdle = true;
        }

        return inRange;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collision detection with player, deal damage
    }
}
