using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PitRespawn : MonoBehaviour
{
    //[SerializeField]
    //private TransitionDoor outDoor;

    //[SerializeField]
    //private GameObject areaExiting;

    //[SerializeField]
    //private GameObject areaEntering;

    [SerializeField]
    private Image screenFade;


    //[SerializeField]
    //private float roomXMin;
    //[SerializeField]
    //private float roomXMax;
    //[SerializeField]
    //private float roomYMin;
    //[SerializeField]
    //private float roomYMax;

    [SerializeField]
    private Vector2 spawnRightPos;

    [SerializeField]
    private Vector2 spawnLeftPos;

    //[SerializeField]
    //private EnemyManager enterEnemyMgr;
    //[SerializeField]
    //private EnemyManager exitEnemyMgr;

    private float damage = 5f;

    private bool respawnLeft = true;

    public Vector3 SpawnLeftPos
    {
        get { return spawnLeftPos; }
    }

    public Vector3 SpawnRightPos
    {
        get { return spawnRightPos; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            /*if (PlayerControllerForces.Instance.IsSleeping)
                return;*/

            PlayerControllerForces.Instance.Sleep(1f);

            if (!PlayerControllerForces.Instance.hasInvincibility)
                PlayerControllerForces.Instance.TakeDamage(damage);

            if (PlayerControllerForces.Instance.currentHP <= 0)
                return;

            StartCoroutine(Transition(collision.collider));
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerControllerForces.Instance.IsSleeping)
                return;

            PlayerControllerForces.Instance.Sleep(1f);

            if (!PlayerControllerForces.Instance.hasInvincibility)
                PlayerControllerForces.Instance.TakeDamage(damage);

            if (PlayerControllerForces.Instance.currentHP <= 0)
                return;

            StartCoroutine(Transition(collision.collider));
            //enterEnemyMgr.SpawnEnemies();
            //exitEnemyMgr.DeleteEnemies();
        }
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entered Trigger");
            float xPos = other.gameObject.transform.position.x;
            if (xPos < this.transform.position.x)
            {
                Debug.Log("Left of Spikes");
                respawnLeft = true;
            }
            else
            {
                Debug.Log("Right of Spikes");
                respawnLeft = false;
            }
        }
    }

    IEnumerator Transition(Collider2D col)
    {
        //if (!areaEntering.activeInHierarchy)
        //{
        //    areaEntering.SetActive(true);
        //    yield return null;
        //}
        //else
        //{
        //    yield return FadeOut(0.5f);
        //    Room Enter = areaEntering.GetComponent<Room>();
        //    Enter.RoomLive = true;
        //    //mainCam.SetBorders(roomXMin, roomXMax, roomYMin, roomYMax);
        //    col.gameObject.transform.position = outDoor.SpawnPos;
        //    //yield return new WaitForSeconds(0.5f);
        //    Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        //    Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        //    yield return Fade(start, target, 0.5f);
        //    yield return FadeIn(0.5f);
        //}


        yield return FadeOut(0.5f);
        if (respawnLeft)
        {
            col.gameObject.transform.position = spawnLeftPos;
        }
        else
        {
            col.gameObject.transform.position = spawnRightPos;
        }
        Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        yield return Fade(start, target, 0.5f);
        yield return FadeIn(0.5f);
    }

    IEnumerator FadeIn(float duration)
    {
        Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 0f);
        
        yield return Fade(start, target, duration);
        //areaExiting.SetActive(false);
        //Room Exit = areaExiting.GetComponent<Room>();
        //Exit.RoomLive = false;
            
    }
    IEnumerator FadeOut(float duration)
    {
        Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 0f);

        yield return Fade(start, target, duration);
    }

    IEnumerator Fade(Color start, Color target, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while(elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            screenFade.color = Color.Lerp(start, target, elapsedPercentage);

            yield return null;

            elapsedTime += Time.deltaTime;
        }
    }
}
