using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCutscene : MonoBehaviour
{
    private BoxCollider2D trigger;
    // Start is called before the first frame update
    void Start()
    {
        trigger = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerControllerForces>() != null)
        {
            SceneManager.LoadScene("Scene Transition Cutscene");
        }
    }
}
