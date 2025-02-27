using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheThrowRope : MonoBehaviour
{
    private float health = 1;
    [SerializeField] public int ropeIndex;

    [SerializeField]
    private ScytheThrowPlatform pf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeDamage(float damage = 1)
    {
        //Remove health
        if ((health - damage) > 0)
        {
        }

        health -= damage;
        Debug.Log("The Damage has been dealt");

        ////Camera shake based off of damage
        //CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);

        //Enemy death calculation
        if (health <= 0)
        {
            PersistentDataManager.Instance.CutPlatform(ropeIndex);
            pf.PlatformFall();
            Destroy(this.gameObject);
        }

    }
}
