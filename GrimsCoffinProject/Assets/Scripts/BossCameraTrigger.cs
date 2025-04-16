using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            CameraShake.Instance.inBossFight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            CameraShake.Instance.inBossFight = false;
        }
    }
}
