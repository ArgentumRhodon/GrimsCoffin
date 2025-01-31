using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool checkForPlayer;

    private bool isColliding = true;
    public bool IsColliding
    {
        get { return isColliding; }
        set { isColliding = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.CompareTag("Enemy"));
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
        //Debug.Log(collision.gameObject.tag);
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
        //Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.layer == 3)
        {
            IsColliding = true;
        }
        if (checkForPlayer && collision.gameObject.tag == "Enemy")
        {
            IsColliding = true;
        }
    }

/*    public bool Grounded()
    {
        return Physics2D.OverlapBox(transform.position, new Vector2(.3f,.3f), 0, groundLayer);
    }*/
}
