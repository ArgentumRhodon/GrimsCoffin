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
    [SerializeField] public Collider2D visionCollider;

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

    //Take damage is 
    public override void TakeDamage(Vector2 knockbackForce, float damage = 1)
    {
        isDamaged = true;
        Sleep(0.5f, knockbackForce);

        //Remove health
        health -= damage;

        //Camera shake based off of damage
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);
/*
        //Enemy death calculation
        if (health <= 0)
            DestroyEnemy();*/
    }
}
