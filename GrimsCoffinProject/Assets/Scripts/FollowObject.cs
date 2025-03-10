using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform followTransform;
    private Vector3 initialDisplacement;

    // Start is called before the first frame update
    void Start()
    {
        initialDisplacement = transform.position - followTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followTransform.position + initialDisplacement;
    }
}