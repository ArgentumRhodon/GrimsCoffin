using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeathScreenBehavior : MonoBehaviour
{
    private bool respawnOnce;

    void Start()
    {
        respawnOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > .8f && !respawnOnce)
        {
            PlayerControllerForces.Instance.Respawn();
            respawnOnce = true;
            DOVirtual.DelayedCall(1, ResetRespawnVar, false);
        }

        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            PlayerControllerForces.Instance.ToggleSleep(false);
            this.gameObject.SetActive(false);
        }
    }

    private void ResetRespawnVar()
    {
        respawnOnce = false;
    }
}
