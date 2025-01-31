using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float visionRange;
    [SerializeField] protected float knockback;
    [SerializeField] protected Collider2D hitbox;

    [Header("GameObjects")]
    [SerializeField] protected Transform enemyGFX;
    [SerializeField] protected GameObject enemy;
    [SerializeField] protected Transform playerTarget;

    //Private references
    protected Seeker seeker;
    protected Rigidbody2D rb;
    private List<Collider2D> collidersDamaged;
    protected bool isPlayerOnRight;

    //Time Variables
    protected float localDeltaTime;
    protected bool isSleeping;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        collidersDamaged = new List<Collider2D>();
        playerTarget = PlayerControllerForces.Instance.gameObject.transform;

        isSleeping = false;
    }

    //Enemy should implement their own update functionality
    protected abstract void FixedUpdate();

    //Destroy enemy, used for when it dies and when it despawns
    public virtual void DestroyEnemy()
    {
        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }

    //Take damage and if below zero, destroy the enemy
    public virtual void TakeDamage(float damage = 1)
    {
        //Delay enemy movement
        CheckPlayerLoc();
        Sleep(0.5f);

        health -= damage;
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);

        if (health <= 0)
            DestroyEnemy();
    }

    protected virtual void Knockback()
    {
        //Check direction for knockback
        int direction;
        if (isPlayerOnRight)
            direction = -1;
        else
            direction = 1;

        rb.velocity = new Vector2(rb.velocity.x * .1f, 0);
        rb.AddForce(new Vector2(direction, 0) * knockback, ForceMode2D.Impulse);
    }

    protected void CheckCollisionWithPlayer()
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

    protected void CheckPlayerLoc()
    {
        if(playerTarget.position.x > transform.position.x)
            isPlayerOnRight = true;
        else
            isPlayerOnRight = false;
    }


    private void Sleep(float duration)
    {
        //Method to help delay time for movement
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private void EndSleep()
    {
        //Method to stop the coroutine from running 
        StopCoroutine(nameof(PerformSleep));
        isSleeping = false;
    }

    private IEnumerator PerformSleep(float duration)
    {
        //Sleeping
        isSleeping = true;

        //Deal knockback impulse
        Knockback();
        yield return new WaitForSecondsRealtime(duration / 8);

        //Reset impulse from combat
        if (knockback != 0)
            rb.velocity = new Vector2(rb.velocity.x * .05f, 0);

        yield return new WaitForSecondsRealtime(duration / 8 * 7);

   
        isSleeping = false;
    }

}
