using Pathfinding;
using Pathfinding.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class WanderingEnemy : Enemy
{
    //Spawn
    private Vector3 spawnLocation;

    //Direction to face
    private int direction = 1;
    private bool isFacingRight;

    //Acceleration
    [Header("Acceleration Rate")]
    [SerializeField] private float walkAcceleration; 
    private float walkAccelAmount; 
    [SerializeField] private float walkDecceleration; 
    private float walkDeccelAmount;

    protected override void Start()
    {
        base.Start();

        spawnLocation = this.transform.position;
        isFacingRight = true;
        direction = 1;

        airChecker.IsColliding = true;

        walkAccelAmount = (50 * walkAcceleration) / movementSpeed;
        walkDeccelAmount = (50 * walkDecceleration) / movementSpeed;
    }

    protected override void FixedUpdate()
    {
        if (!isSleeping)
        {
            UpdatePath();          
        }
        CheckCollisionWithPlayer();
    }
    private void UpdatePath()
    {
        CheckFlip();

        if (!isSleeping)
        {
            Walk(1);
        }
    }

    private void CheckFlip()
    {
        //Check to see if it is not colliding with the ground or is colliding with a wall
        //Debug.Log($"Air Checker: {airChecker.Grounded()} Wall Checker: {wallChecker.Grounded()}");
        if (!airChecker.IsColliding || wallChecker.IsColliding)
        //if (!airChecker.Grounded() || wallChecker.Grounded())
        {
/*            //Check if the enemy is falling
            if (airChecker.IsColliding && !wallChecker.IsColliding)
            {
                return;
            }*/

            Turn();
            airChecker.IsColliding = true;
            wallChecker.IsColliding = false;
        }
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
        direction *= -1;
    }

    private void Walk(float lerpAmount)
    {
        //Calculate the direction and our desired velocity
        float targetSpeed = direction * movementSpeed;
        //float targetSpeed = moveInput.x * Data.walkMaxSpeed; <---------- used for walking at a slower pace
        //Smooth changes to direction and speed using a lerp function
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? walkAccelAmount : walkDeccelAmount;

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

        //Convert movement to a vector and apply it
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

}
