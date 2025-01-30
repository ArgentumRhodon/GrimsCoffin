using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScytheProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance;
    [SerializeField] private float damage;
    private float distance;
    private Vector3 direction;

    private CircleCollider2D hitbox;
    private Animator animator;
    private SpriteRenderer sprite;

    public bool facingRight;
    private bool returning;

    private void Start()
    {
        hitbox = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        facingRight = PlayerControllerForces.Instance.playerState.IsFacingRight;

        if (!facingRight)
        {
            direction = Vector3.left;
            FlipScythe();
        }

        else
            direction = Vector3.right;

        distance = 0;
        returning = false;
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        float travel = Time.deltaTime * speed;
        if (!returning)
        {
            this.transform.parent.transform.Translate(direction * travel);
            distance += travel;

            returning = distance >= maxDistance;
            if (returning)
                FlipScythe();
        }

        else
        {
            Vector3 destination = PlayerControllerForces.Instance.gameObject.transform.position;

            this.transform.parent.transform.position = Vector3.MoveTowards(this.transform.parent.transform.position, destination, speed * Time.deltaTime * 1.25f);
        }
    }

    private void FlipScythe()
    {
        sprite.flipX = !sprite.flipX;
        float _direction = animator.GetFloat("direction");
    }

    private void OnEnable()
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        PlayerControllerForces.Instance.scytheThrown = false;
        Destroy(this.transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {  
        if (collision.gameObject.layer == 3)
        {
            distance = maxDistance;
        }

        if (collision.gameObject.GetComponent<TeamComponent>() != null)
        {
            if (collision.gameObject.GetComponent<TeamComponent>().teamIndex == TeamIndex.Enemy)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(Vector2.zero, damage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (returning && collision.gameObject.GetComponent<PlayerControllerForces>() != null)
            enabled = false;
    }
}
