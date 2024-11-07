using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float visionRange;

    [Header("GameObjects")]
    [SerializeField] protected Transform enemyGFX;
    [SerializeField] protected GameObject enemy;

    //Private references
    protected Seeker seeker;
    protected Rigidbody2D rb;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    //Enemy should implement their own update functionality
    protected abstract void FixedUpdate();

    //Destroy enemy, used for when it dies and when it despawns
    public virtual void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    //Take damage and if below zero, destroy the enemy
    public virtual void TakeDamage(float damage = 1)
    {
        health -= damage;

        if (health < 0)
            DestroyEnemy();
    }

}
