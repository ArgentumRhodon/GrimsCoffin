using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    private Vector3 startingPosition;
    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.gameObject.activeInHierarchy)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .05f, transform.position.z);
        }

        else
        {
            this.transform.position = startingPosition;
        }
    }
}
