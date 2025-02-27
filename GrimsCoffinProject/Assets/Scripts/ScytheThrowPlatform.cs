using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheThrowPlatform : MonoBehaviour
{
    private float health = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlatformFall()
    {
        this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
