using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTEnemy : Enemy
{
    //Direction to face
    private int direction = 1;
    [SerializeField] private bool isFacingRight;
    

    public int Direction { get { return direction; } set { direction = value; } }
    public bool IsFacingRight { get { return isFacingRight; } set { isFacingRight = value; } }

    //Direction Checkers
    [Header("Direction Collision Checkers")]
    [SerializeField] public GroundChecker wallChecker;
    [SerializeField] public GroundChecker airChecker;
    [SerializeField] public Collider2D visionCollider;
    [SerializeField] public Collider2D attackCollider;

    //Acceleration
    [Header("Acceleration Rate")]
    [SerializeField] public float movementAcceleration;
    [HideInInspector] public float movementAccelAmount;
    [SerializeField] public float movementDecceleration;
    [HideInInspector] public float movementDeccelAmount;

    private Canvas enemyCanvas;

    protected override void Start()
    {
        base.Start();
        isFacingRight = true;
        direction = 1;

        movementAccelAmount = (1 * movementAcceleration) / movementSpeed;
        movementDeccelAmount = (1 * movementDecceleration) / movementSpeed;

        enemyCanvas = gameObject.GetComponentInChildren<Canvas>();
    }

    protected override void FixedUpdate()
    {
        CheckCollisionWithPlayer();
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
    }

    //Damage player if colliding with the enemy
    public override void CheckCollisionWithPlayer()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(attackCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Player && !PlayerControllerForces.Instance.hasInvincibility
                    && !PlayerControllerForces.Instance.hasDashInvincibility)
                {
                    PlayerControllerForces.Instance.TakeDamage(damage);
                }
            }
        }
    }

    public void FaceRight(bool shouldFaceRight = true)
    {
        //Debug.Log("Is turning: " + shouldFaceRight);

        //Transform local scale of object
        Vector3 scale = transform.localScale;
        scale.x = shouldFaceRight ? Mathf.Abs(scale.x) : -1 * Mathf.Abs(scale.x);
        transform.localScale = scale;

        //Updates scale of UI so that it is always facing right
        Vector3 tempScale = enemyCanvas.transform.localScale;
        tempScale.x = shouldFaceRight ? Mathf.Abs(tempScale.x) : -1 * Mathf.Abs(tempScale.x);       
        enemyCanvas.transform.localScale = tempScale;

        IsFacingRight = shouldFaceRight;
        Direction = shouldFaceRight ? Mathf.Abs(Direction) : -1 * Mathf.Abs(Direction);       
    }
}
