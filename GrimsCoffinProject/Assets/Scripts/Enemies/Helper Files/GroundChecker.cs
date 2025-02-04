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

    public void ReUpdateTrigger()
    {
        //Debug.Log("Reupdating Trigger");

        transform.position = new Vector2(transform.position.x + 0.001f, transform.position.y);

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;

        //Debug.Log("Is colliding?: " + isColliding);

        /*        LayerMask m_LayerMask = new LayerMask();
                Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);

                Debug.Log(hitColliders.Length);

                //Check when there is a new collider coming into contact with the box
                for(int i = 0; i < hitColliders.Length; i++)
                {
                    Debug.Log("Hit : " + hitColliders[i].name + i);

                    if (hitColliders[i].gameObject.layer == 3)
                    {
                        IsColliding = true;
                        return;
                    }
                    else if (checkForPlayer && hitColliders[i].gameObject.tag == "Enemy")
                    {
                        IsColliding = true;
                        return;
                    }
                    else
                    {
                        IsColliding = false;
                    }
                }*/
    }

/*    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Trigger Stay is running");
        if (collision.gameObject.layer == 3)
        {
            IsColliding = true;
        }
        if (checkForPlayer && collision.gameObject.tag == "Enemy")
        {
            IsColliding = true;
        }
    }*/
}
