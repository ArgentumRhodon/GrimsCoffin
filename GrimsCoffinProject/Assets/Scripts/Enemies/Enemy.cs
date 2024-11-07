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
    [SerializeField] protected float visibilityRange;

    [Header("GameObjects")]
    [SerializeField] protected Transform enemyGFX;
    [SerializeField] protected GameObject enemy;

    //Private 
    protected Seeker seeker;
    protected Rigidbody2D rb;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected abstract void FixedUpdate();

    public virtual void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

}
