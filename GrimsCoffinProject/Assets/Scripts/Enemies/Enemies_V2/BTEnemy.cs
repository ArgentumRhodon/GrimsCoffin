using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTEnemy : Enemy
{
    //Direction to face
    private int direction = 1;
    private bool isFacingRight;

    public int Direction { get { return direction; } set { direction = value; } }
    public bool IsFacingRight { get { return isFacingRight; } set { isFacingRight = value; } }

    //Direction Checkers
    [Header("Direction Collision Checkers")]
    [SerializeField] public GroundChecker wallChecker;
    [SerializeField] public GroundChecker airChecker;

    //Acceleration
    [Header("Acceleration Rate")]
    [SerializeField] public float walkAcceleration;
    [HideInInspector] public float walkAccelAmount;
    [SerializeField] public float walkDecceleration;
    [HideInInspector] public float walkDeccelAmount;

    protected override void Start()
    {
        base.Start();
        isFacingRight = true;
        direction = 1;

        airChecker.IsColliding = true;

        walkAccelAmount = (1 * walkAcceleration) / movementSpeed;
        walkDeccelAmount = (1 * walkDecceleration) / movementSpeed;
    }

    protected override void FixedUpdate()
    {
    }
}
