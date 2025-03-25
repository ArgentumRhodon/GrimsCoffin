using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private BoxCollider2D trigger;

    private float damage = 5;

    private float health = 1;

    [SerializeField]
    public EventReference oneShotFX;
    [SerializeField]
    public EventReference oneShotFX2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Entered");
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 5;
            Destroy(trigger.gameObject);
            RuntimeManager.PlayOneShotAttached(oneShotFX, this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerControllerForces.Instance.TakeDamage(damage);
        }
        RuntimeManager.PlayOneShotAttached(oneShotFX2,this.gameObject);
            Destroy(this.gameObject);
    }

    public void TakeDamage(float damage = 1)
    {
        //Remove health
        if ((health - damage) > 0)
        {
            RuntimeManager.PlayOneShotAttached(oneShotFX, this.gameObject);
        }

        health -= damage;
        Debug.Log("The Damage has been dealt");

        //Camera shake based off of damage
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);

        //Instantiate(hitEffect, this.transform.position, Quaternion.identity);

        //Enemy death calculation
        //if (health <= 0)
        //{
        //    breakInstance.start();
        Destroy(this.gameObject);
        //}


    }
}
