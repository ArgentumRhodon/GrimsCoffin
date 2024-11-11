using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float visionRange;
    [SerializeField] protected Collider2D hitbox;

    [Header("GameObjects")]
    [SerializeField] protected Transform enemyGFX;
    [SerializeField] protected GameObject enemy;
    [SerializeField] protected Transform playerTarget;

    //Private references
    protected Seeker seeker;
    protected Rigidbody2D rb;
    private List<Collider2D> collidersDamaged;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        collidersDamaged = new List<Collider2D>();
        playerTarget = PlayerControllerForces.Instance.gameObject.transform;
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
        health -= damage;

        if (health <= 0)
            DestroyEnemy();
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

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Player && !PlayerControllerForces.Instance.hasInvincibility)
                {
                    PlayerControllerForces.Instance.TakeDamage(damage);
                }
            }
        }
    }

}
