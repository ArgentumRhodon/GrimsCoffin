using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionDoor : MonoBehaviour
{
    [SerializeField]
    private TransitionDoor outDoor;

    [SerializeField]
    private GameObject areaExiting;

    [SerializeField]
    private PolygonCollider2D ColliderEntering;

    [SerializeField] 
    private CinemachineConfiner FollowCameraConfiner;

    [SerializeField]
    private GameObject areaEntering;

    [SerializeField]
    private Image screenFade;

    [SerializeField]
    private CameraFollow mainCam;

    [SerializeField]
    private float roomXMin;
    [SerializeField]
    private float roomXMax;
    [SerializeField]
    private float roomYMin;
    [SerializeField]
    private float roomYMax;

    private Vector3 spawnPos;

    [SerializeField]
    private EnemyManager enterEnemyMgr;
    [SerializeField]
    private EnemyManager exitEnemyMgr;
    [SerializeField]
    private GameObject enemyDropList;

    //FMOD Related Variables
    #region FMODRelated

    [SerializeField] private EventReference idleSFX;
    [SerializeField] private EventReference testSFX;
    private EventInstance idleInstance;


    #endregion

    public Vector3 SpawnPos
    {
        get { return spawnPos; }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.GetChild(0).position;
        if (idleSFX.IsNull != true) {
            idleInstance = RuntimeManager.CreateInstance(idleSFX);
            idleInstance.start();
        }
        enemyDropList = GameObject.Find("Enemy Drops");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Transition(collision));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerControllerForces.Instance.Sleep(1.5f);
            if (areaEntering.GetComponent<Room>().roomIndex == 2 && SceneManager.GetActiveScene().name == "OnboardingLevel")
            {
                PlayerControllerForces.Instance.Data.canDash = true;
            }
            else if(areaEntering.GetComponent<Room>().roomIndex == 3 && SceneManager.GetActiveScene().name == "OnboardingLevel")
            {
                PlayerControllerForces.Instance.Data.canDoubleJump = true;
                PlayerControllerForces.Instance.Data.canWallJump = true;
            }

            if (enterEnemyMgr != null)
            {
                Debug.Log("Spawning Enemies");
                enterEnemyMgr.SpawnEnemies();
            }
                

            if (exitEnemyMgr != null)
            {
                Debug.Log("Deleting Enemies");
                exitEnemyMgr.DeleteEnemies();
            }

            if (enemyDropList != null)
            {
                foreach (Transform child in enemyDropList.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    IEnumerator Transition(Collider2D col)
    {
        
        if (!areaEntering.activeInHierarchy)
        {
            areaEntering.SetActive(true);
            yield return null;
            idleInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        else
        {
            yield return FadeOut(0.5f);
            Room Enter = areaEntering.GetComponent<Room>();
            Enter.RoomLive = true;
            //mainCam.SetBorders(roomXMin, roomXMax, roomYMin, roomYMax);
            col.gameObject.transform.position = outDoor.SpawnPos;
            //ColliderEntering.gameObject.SetActive(true);
            FollowCameraConfiner.m_BoundingShape2D = ColliderEntering;
            //yield return new WaitForSeconds(0.5f);
            Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
            Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
            yield return Fade(start, target, 0.5f);
            yield return FadeIn(0.5f);
        }
    }

    IEnumerator FadeIn(float duration)
    {
        Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 0f);
        
        yield return Fade(start, target, duration);
        areaExiting.SetActive(false);
        Room Exit = areaExiting.GetComponent<Room>();
        Exit.RoomLive = false;
            
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
