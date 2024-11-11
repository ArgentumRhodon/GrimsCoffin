using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private float nextWaypointDistance = 3f;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    protected override void Start()
    {
        base.Start();

        //Square range at start to make it easier to calculate later
        visionRange = visionRange * visionRange;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
        //seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    protected override void FixedUpdate()
    {
        if (!isSleeping)
        {
            PathFollow();
        }

        CheckCollisionWithPlayer();
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

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * movementSpeed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(1, 1, 1);
        }
        else if (force.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(-1, 1, 1);
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
            float distance = Mathf.Pow((playerTarget.transform.position.x - enemy.transform.position.x), 2)
                                + Mathf.Pow((playerTarget.transform.position.y - enemy.transform.position.y), 2);

            if (distance < visionRange)
                seeker.StartPath(rb.position, playerTarget.position, OnPathComplete);
        }
    }
}
