using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTEnemy : Enemy
{
    private Canvas enemyCanvas;
    protected PlayerControllerForces player;
    [HideInInspector] public EnemyStateList enemyStateList;

    //Direction to face
    private int direction = 1;
    public int Direction { get { return direction; } set { direction = value; } }

    //Direction Checkers
    [Header("Direction Collision Checkers")]
    [SerializeField] public GroundChecker wallChecker;
    [SerializeField] public GroundChecker airChecker;
    [SerializeField] public Collider2D visionCollider;
    [SerializeField] protected Collider2D bodyCollider;
    [SerializeField] public Collider2D attackCollider;

    //Acceleration
    [Header("Acceleration Rate")]
    [SerializeField] public float movementAcceleration;
    [HideInInspector] public float movementAccelAmount;
    [SerializeField] public float movementDecceleration;
    [HideInInspector] public float movementDeccelAmount;

    protected override void Start()
    {
        base.Start();
        enemyCanvas = gameObject.GetComponentInChildren<Canvas>();
        player = PlayerControllerForces.Instance;
        enemyStateList = gameObject.GetComponent<EnemyStateList>();

        enemyStateList.IsFacingRight = true;
        direction = 1;

        movementAccelAmount = (1 * movementAcceleration) / movementSpeed;
        movementDeccelAmount = (1 * movementDecceleration) / movementSpeed;
    }

    protected override void FixedUpdate()
    {
        CheckCollisionWithPlayer();
        CheckCollisionWithPlayer(attackCollider);
    }

    //Take damage is 
    public override void TakeDamage(Vector2 knockbackForce, float damage = 1)
    {
        isDamaged = true;

        if (canBeStopped)
            Sleep(0.5f, knockbackForce);

        //Remove health
        health -= damage;

        //Camera shake based off of damage
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);
    }

/*    //Damage player if colliding with the enemy
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
    }*/

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

        enemyStateList.IsFacingRight = shouldFaceRight;
        Direction = shouldFaceRight ? Mathf.Abs(Direction) : -1 * Mathf.Abs(Direction);       
    }

    public void TurnToPlayer()
    {
        //Find direction and update it
        Vector2 direction = FindPlayerDirection();

        if (direction.x > 0 && !enemyStateList.IsFacingRight)
        {
            FaceRight(true);
        }
        else if (direction.x < 0 && enemyStateList.IsFacingRight)
        {
            FaceRight(false);
        }
    }

    public float FindPlayerDistanceX()
    {
        return Mathf.Abs(player.transform.position.x - transform.position.x);
    }    
    
    public float FindPlayerDistanceY()
    {
        return Mathf.Abs(player.transform.position.y - transform.position.y);
    }

/*    public Vector2 FindAerialPlayerDistance()
    {
        return new Vector2(Mathf.Abs(player.transform.position.x - transform.position.x), Mathf.Abs(player.transform.position.y - transform.position.y));
    }*/

    public Vector2 FindPlayerDirection()
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y + 1);
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);

        return (playerPos - enemyPos).normalized;
    }

    public int GetPlayerXDirection()
    {
        return player.transform.position.x > transform.position.x ? 1 : -1;
    }
}
