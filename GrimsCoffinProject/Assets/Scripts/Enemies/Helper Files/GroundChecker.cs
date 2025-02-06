using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool checkForPlayer;

    [SerializeField] private bool isColliding;
    public bool IsColliding
    {
        get { return isColliding; }
        set { isColliding = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Updated enter trigger");
        if (collision.gameObject.layer == 3)
        {
            IsColliding = true;
        }
        if(checkForPlayer && collision.gameObject.tag == "Enemy")
        {
            IsColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //3 is the layer for ground
        //Debug.Log("Updated exit trigger");
        if (collision.gameObject.layer == 3)
        {
            IsColliding = false;
        }
        if (checkForPlayer && collision.gameObject.tag == "Enemy")
        {
            IsColliding = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Updated enter trigger");
        if (collision.gameObject.layer == 3)
        {
            IsColliding = true;
        }
        if (checkForPlayer && collision.gameObject.tag == "Enemy")
        {
            IsColliding = true;
        }
    }

    public void ReUpdateTrigger()
    {
        //Debug.Log("Reupdating Trigger");

        transform.position = new Vector2(transform.position.x + 0.001f, transform.position.y);

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;

    }
}
