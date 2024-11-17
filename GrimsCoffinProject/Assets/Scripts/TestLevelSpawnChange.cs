using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestLevelSpawnChange : MonoBehaviour
{
    [SerializeField]
    private GameObject jumpSpawn;

    [SerializeField]
    private GameObject dashSpawn;

    [SerializeField]
    private GameObject wallSpawn;

    [SerializeField]
    private PlayerControllerForces player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            player.transform.position = jumpSpawn.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            player.transform.position = dashSpawn.transform.position;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            player.transform.position = wallSpawn.transform.position;
        }
    }
}
