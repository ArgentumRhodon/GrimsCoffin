using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float health;
    [SerializeField] public float damage;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float seekSpeed;
    [SerializeField] public float visionRange;
    [SerializeField] [Range(0f, 5f)] protected float knockbackMult;
    [SerializeField] protected Collider2D hitbox;
    [SerializeField] private bool canBePulledDown;

    [Header("GameObjects")]
    [SerializeField] protected Transform enemyGFX;
    [SerializeField] protected GameObject enemy;
    [SerializeField] protected Transform playerTarget;
    [SerializeField] protected GameObject enemyDropPrefab;
    [SerializeField] protected GameObject enemyDropList;

    //Private references
    protected Seeker seeker;
    protected Rigidbody2D rb;
    protected List<Collider2D> collidersDamaged;
    protected bool isPlayerOnRight;

    //Time Variables
    protected float localDeltaTime;
    protected bool isSleeping;

    //DownAttack
    protected bool isHitDown;

    //Damaged checked for conditions
    protected bool isDamaged;
    public bool IsDamaged { get { return isDamaged; } set { isDamaged = value; } }

    //Positions used for state checks
    [Header("Tile Checks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);


    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Get components for later reference
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        collidersDamaged = new List<Collider2D>();
        playerTarget = PlayerControllerForces.Instance.gameObject.transform;

        enemyDropList = GameObject.Find("Enemy Drops");

        isSleeping = false;
    }

    //Enemy should implement their own update functionality
    protected abstract void FixedUpdate();

    //Take damage and if below zero, destroy the enemy
    public virtual void TakeDamage(Vector2 knockbackForce, float damage = 1)
    {
        //Delay enemy movement
        CheckPlayerLoc();
        Sleep(0.5f, knockbackForce);

        //Remove health
        health -= damage;

        //Camera shake based off of damage
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);

        //Enemy death calculation
        if (health <= 0)
            DestroyEnemy();
    }

    //Damage player if colliding with the enemy
    public virtual void CheckCollisionWithPlayer()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitbox, filter, collidersToDamage);

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

    //Helper methods ----------------------------------------------

    //Check player location, used when player takes damage (may not be needed since knockback is calculated in the combat scripts)
    protected void CheckPlayerLoc()
    {
        if(playerTarget.position.x > transform.position.x)
            isPlayerOnRight = true;
        else
            isPlayerOnRight = false;
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

    public virtual void RemoveActiveEnemy()
    {
        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
    }

    public virtual void DestroyEnemyGO()
    {
        if (Random.Range(1, 100) <= 50)
        {
            GameObject drop = Instantiate(enemyDropPrefab, enemyDropList.transform);
            drop.transform.position = this.transform.position;
        }
        Destroy(this.gameObject);
    }

    //Add knockback to the enemy based off a given force
    public virtual void Knockback(Vector2 knockbackForce)
    {
        //Reset current velocity to make sure it doesn't stack
        rb.velocity = new Vector2(rb.velocity.x * .1f, 0);

        //Impulse force using the knockbackForce parameter, consider knockbackMult, don't apply knockback if 0
        rb.AddForce(knockbackForce * knockbackMult, ForceMode2D.Impulse);
    }

    public bool Grounded()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
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

        //Deal knockback impulse
        Knockback(knockbackForce);
        yield return new WaitForSecondsRealtime(duration / 8);

        //Reset impulse from combat
        if (knockbackMult != 0)
            rb.velocity = new Vector2(rb.velocity.x * .05f, 0);

        yield return new WaitForSecondsRealtime(duration / 8 * 7);
   
        isSleeping = false;
    }

    //Wall collision check gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}
