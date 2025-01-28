using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class Wander : ActionNode
{
    //Speed
    [Tooltip("How fast to move")]
    public NodeProperty<float> movementSpeed = new NodeProperty<float> { defaultValue = 5.0f };

    //Stopping Distance
    [Tooltip("Stop within this distance of the target")]
    public NodeProperty<float> stoppingDistance = new NodeProperty<float> { defaultValue = 0.1f };

    //Update Rotation
    [Tooltip("Updates the agents rotation along the path")]
    public NodeProperty<bool> updateRotation = new NodeProperty<bool> { defaultValue = true };

    //Acceleration
    [Tooltip("Maximum acceleration when following the path")]
    public NodeProperty<float> acceleration = new NodeProperty<float> { defaultValue = 40.0f };

    [Tooltip("Maximum deacceleration when following the path")]
    public NodeProperty<float> walkDecceleration = new NodeProperty<float> { defaultValue = 40.0f };

    private float walkAccelAmount;
    private float walkDeccelAmount;

    //Checkpoint Tolerance
    [Tooltip("Returns success when the remaining distance is less than this amount")]
    public NodeProperty<float> tolerance = new NodeProperty<float> { defaultValue = 1.0f };

    //Target Position
    [Tooltip("Target Position")]
    public NodeProperty<Vector3> targetPosition = new NodeProperty<Vector3> { defaultValue = Vector3.zero };


    //Direction Checkers
    [SerializeField] private GroundChecker wallChecker;
    [SerializeField] private GroundChecker airChecker;

    //Direction to face
    private int direction;
    private bool isFacingRight;

    protected override void OnStart() {
        isFacingRight = true;
        direction = 1;

        //wallChecker = GetComponent<Enemy>().wallChecker;
        //wallChecker = this.GameObject.GetComponent<Enemy>();

        airChecker.IsColliding = true;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() 
    {

        if (context.agent.pathPending)
        {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance.Value)
        {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }

    private void Walk(float lerpAmount)
    {
        /*//Calculate the direction and our desired velocity
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
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);*/
    }
}
