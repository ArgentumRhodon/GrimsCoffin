using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenBehavior : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            PlayerControllerForces.Instance.Respawn();
            this.gameObject.SetActive(false);
        }
    }
}
