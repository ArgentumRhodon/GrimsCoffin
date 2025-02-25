using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;
using BehaviorDesigner.Runtime;

public abstract class Enemy : MonoBehaviour
{
    //Data & Variables --------------------------------------------------------------------------------------------
    #region Data & Variables
    [Header("Stats")]
    //General stats -----------------------------------------------------------------------------------------------
    [SerializeField] public float health;
    [SerializeField] public float visionRange;

    //Damage ------------------------------------------------------------------------------------------------------
    [SerializeField] public float collisionDamage;
    [SerializeField] public bool damageOnCollision;
    [SerializeField][Range(0f, 5f)] protected float knockbackMult;

    //Tracks hits
    [SerializeField] private float hurtDodgeMin; //Inclusive
    [SerializeField] private float hurtMaxTimer;
    private float hurtInSuccessionTotal;
    private float hurtSuccessionTimer;

    public float HurtDodgeMin { get { return hurtDodgeMin; } set { hurtDodgeMin = value; } }
    public float HurtInSuccessionTotal { get { return hurtInSuccessionTotal; } set { hurtInSuccessionTotal = value; } }
    public float HurtSuccessionTimer { get { return hurtSuccessionTimer; } set { hurtSuccessionTimer = value; } }

    //Private attack damage updated in behavior trees - TODO: May not need this with restructure of the enemy class
    private float attackDamage;
    public float AttackDamage { get { return attackDamage; } set { attackDamage = value; } }

    public float[] attackDamages;


    //Movement ----------------------------------------------------------------------------------------------------
    [Header("Movement")]
    [SerializeField] public float movementSpeed;
    [SerializeField] public float seekSpeed;

    [SerializeField] public float movementAcceleration;
    [HideInInspector] public float movementAccelAmount;
    [SerializeField] public float movementDeacceleration;
    [HideInInspector] public float movementDeaccelAmount;

    //Attack physics and stats ------------------------------------------------------------------------------------
    [SerializeField] private bool canBePulledDown;
    [SerializeField] protected bool canBeStopped = true;
    public bool CanBeStopped { get { return canBeStopped; } set { canBeStopped = value; } }

    //Enemy Statuses ----------------------------------------------------------------------------------------------
    protected bool isSleeping;
    protected bool isHitDown;
    protected bool isBlocking;
    protected bool isStaggered;
    protected bool isDamaged;
    protected bool isDead;
    private int direction = 1;

    //Public get/setters
    public bool IsBlocking { get { return isBlocking; } set { isBlocking = value; } }
    public bool IsStaggered { get { return isStaggered; } set { isStaggered = value; } }
    public bool IsDamaged { get { return isDamaged; } set { isDamaged = value; } }
    public int Direction { get { return direction; } set { direction = value; } }


    [Header("GameObject References")] // --------------------------------------------------------------------------
    [SerializeField] protected Transform enemyGFX;
    [SerializeField] protected GameObject enemy;
    [SerializeField] protected Transform playerTarget;
    [SerializeField] protected GameObject enemyDropPrefab;
    [SerializeField] protected GameObject enemyDropList;

    //Private references ------------------------------------------------------------------------------------------
    protected Seeker seeker;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected List<Collider2D> collidersDamaged;
    protected bool isPlayerOnRight;
    protected Canvas enemyCanvas;
    protected SpriteRenderer spriteRenderer;
    protected PlayerControllerForces player;
    [HideInInspector] public EnemyStateList enemyStateList;
    [HideInInspector] public BehaviorTree behaviorTree;

    [Header("Collision Checkers & Associated Variables")] // ------------------------------------------------------
    [Space(5)]

    //Positions used for state checks -----------------------------------------------------------------------------
    [Header("Ground Tile Checks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);

    //Direction Checkers ------------------------------------------------------------------------------------------
    [Header("Direction Collision Checkers")]
    [SerializeField] public GroundChecker wallChecker;
    [SerializeField] public GroundChecker airChecker;
    [SerializeField] public Collider2D visionCollider;
    [SerializeField] public Collider2D kinematicCollider;

    //Attack Colliders --------------------------------------------------------------------------------------------
    [Header("Attack Collision")]
    [SerializeField] protected Collider2D bodyCollider;
    [SerializeField] public Collider2D attackCollider;

    [SerializeField] protected bool canDamageWhenColliding;
    #endregion

    //Runtime Methods ---------------------------------------------------------------------------------------------
    #region Runtime
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Get components for later reference
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        enemyCanvas = gameObject.GetComponentInChildren<Canvas>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        enemyStateList = gameObject.GetComponent<EnemyStateList>();
        behaviorTree = GetComponent<BehaviorTree>();

        //Make sure kinematicCollider doesn't do weird things
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), kinematicCollider);
        Physics2D.IgnoreCollision(attackCollider, kinematicCollider);

        //Player refs
        playerTarget = PlayerControllerForces.Instance.gameObject.transform;
        player = PlayerControllerForces.Instance;

        //Set new collider
        collidersDamaged = new List<Collider2D>();

        //Enemy drop list
        enemyDropList = GameObject.Find("Enemy Drops");

        //Set states and variables
        isSleeping = false;
        isDead = false;

        enemyStateList.IsFacingRight = true;
        direction = 1;

        movementAccelAmount = (1 * movementAcceleration) / movementSpeed;
        movementDeaccelAmount = (1 * movementDeacceleration) / movementSpeed;     
    }

    //Enemy should implement their own update functionality
    protected virtual void FixedUpdate()
    {
        if(damageOnCollision)
            CheckCollisionWithPlayer();

        CheckCollisionWithPlayer(attackCollider, 10); //TODO: Have specific classes for different enemy types

        if (hurtInSuccessionTotal > 0)
            hurtSuccessionTimer -= Time.deltaTime;
        else
        {
            hurtInSuccessionTotal = 0;
        }
    }
    #endregion

    //Damage, Destruction, and Health Methods ---------------------------------------------------------------------
    #region Damage, Destruction, and Health Methods
    //Damage player if colliding with the enemy
    public virtual void CheckCollisionWithPlayer()
    {
        if (isDead) return;
        if (!canDamageWhenColliding) return;

        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(bodyCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Player && !PlayerControllerForces.Instance.hasInvincibility
                    && !PlayerControllerForces.Instance.hasDashInvincibility)
                {
                    PlayerControllerForces.Instance.TakeDamage(collisionDamage);
                }
            }
        }
    }
    public virtual void CheckCollisionWithPlayer(Collider2D hitboxToCheck, float damage)
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitboxToCheck, filter, collidersToDamage);

        Debug.Log("This is running");

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();
                //Debug.Log(hitTeamComponent);

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Player && !PlayerControllerForces.Instance.hasInvincibility
                    && !PlayerControllerForces.Instance.hasDashInvincibility)
                {
                    PlayerControllerForces.Instance.TakeDamage(damage);
                }
            }
        }
    }
    
    //Take damage and if below zero, destroy the enemy
    public virtual void TakeDamage(Vector2 knockbackForce, float damage = 1)
    {
        //Delay enemy movement ----- TODO: Check to see if I even need this with the knockbackForce
        CheckPlayerLoc();

        //If the enemy can be stopped, sleep and take a knockback force
        if (canBeStopped)
        {
            EndSleep();
            Sleep(.35f, knockbackForce);
            isStaggered = true;
        }    

        //If the enemy is blocking, don't take damage
        if (IsBlocking)
            return;

        //Remove health
        health -= damage;

        //Camera shake based off of damage
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);

        //Tracker for enemies that can dodge
        hurtSuccessionTimer = hurtMaxTimer;
        hurtInSuccessionTotal++;

        //Enemy death calculation - TODO: add delayed call for destroy enemy
/*        if (health <= 0)
            DestroyEnemy();*/
    }

    //Add knockback to the enemy based off a given force
    public virtual void Knockback(Vector2 knockbackForce)
    {
        //Reset current velocity to make sure it doesn't stack
        rb.velocity = new Vector2(rb.velocity.x * .1f, 0);

        //Impulse force using the knockbackForce parameter, consider knockbackMult, don't apply knockback if 0
        rb.AddForce(knockbackForce * knockbackMult, ForceMode2D.Impulse);
    }

    //Destroy enemy, used for when it dies and when it despawns
    public virtual void DestroyEnemy()
    {
        if (Random.Range(1, 100) <= 50)
        {
            GameObject drop = Instantiate(enemyDropPrefab, enemyDropList.transform);
            drop.transform.position = this.transform.position;
        }

        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }

    //Remove enemy from the room pool
    public virtual void RemoveActiveEnemy()
    {
        isDead = true;
        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
    }

    //Destroy the game object
    public virtual void DestroyEnemyGO()
    {
        //Random values for drops
        if (Random.Range(1, 100) <= 50)
        {
            GameObject drop = Instantiate(enemyDropPrefab, enemyDropList.transform);
            drop.transform.position = this.transform.position;
        }
        Destroy(this.gameObject);
    }

    #endregion

    //Helper Methods ----------------------------------------------------------------------------------------------
    #region Helper Methods
    //Checks to see if the enemy is currently grounded
    public bool Grounded()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
    }

    //Faces specific direction based off of passed in parameter and updates all associated values
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

    //Sleep methods to run, end, and execute the sleep coroutine
    public void Sleep(float duration, Vector2 knockbackForce)
    {
        //Method to help delay time for movement
        StartCoroutine(PerformSleep(duration, knockbackForce));
    }

    public void EndSleep()
    {
        //Method to stop the coroutine from running 
        StopCoroutine(nameof(PerformSleep));
        isSleeping = false;
    }

    private IEnumerator PerformSleep(float duration, Vector2 knockbackForce)
    {
        //Sleeping
        isSleeping = true;
        behaviorTree.enabled = false;
        animator.speed = 0;
        spriteRenderer.color = Color.red;

        //Deal knockback impulse
        Knockback(knockbackForce);
        yield return new WaitForSecondsRealtime(duration / 8);

        //Reset impulse from combat
        if (knockbackMult != 0)
            rb.velocity = new Vector2(rb.velocity.x * .05f, 0);

        yield return new WaitForSecondsRealtime(duration / 8 * 7);

        animator.speed = 1f;
        behaviorTree.enabled = true;
        spriteRenderer.color = Color.white;
        isSleeping = false;
    }

/*    //Toggle if the Rigidbody is kinematic or not
    public void ToggleRigidBody(bool isKinematic)
    {
        if (isKinematic)
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        gameObject.GetComponent<Rigidbody2D>().isKinematic = isKinematic;
    }*/


    #endregion

    //Player Checks & Methods -------------------------------------------------------------------------------------
    #region Player Checks & Methods  
    //Check player location, used when player takes damage (may not be needed since knockback is calculated in the combat scripts)
    protected void CheckPlayerLoc()
    {
        if (playerTarget.position.x > transform.position.x)
            isPlayerOnRight = true;
        else
            isPlayerOnRight = false;
    }

    //Checks direction of the player and turns to them
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

    //Find the distance of enemy to player ------------------------------------------------------------------------
    public float FindPlayerDistanceX()
    {
        return Mathf.Abs(player.transform.position.x - transform.position.x);
    }

    public float FindPlayerDistanceY()
    {
        return Mathf.Abs(player.transform.position.y - transform.position.y);
    }

    public Vector2 FindPlayerDirection()
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y + 1);
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);

        return (playerPos - enemyPos).normalized;
    }

    /*    public Vector2 FindAerialPlayerDistance()
        {
            return new Vector2(Mathf.Abs(player.transform.position.x - transform.position.x), Mathf.Abs(player.transform.position.y - transform.position.y));
        }*/

    //Get the direction of the player to enemy in the X vector
    public int GetPlayerXDirection()
    {
        return player.transform.position.x > transform.position.x ? 1 : -1;
    }
    #endregion

    //Wall collision check gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}
