using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenBehavior : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > .7f)
        {
            PlayerControllerForces.Instance.Respawn();
        }

        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            PlayerControllerForces.Instance.ToggleSleep(false);
            this.gameObject.SetActive(false);
        }
           
    }
}
