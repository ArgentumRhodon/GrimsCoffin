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

    [SerializeField]
    public EventReference oneShotFX;
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
}
