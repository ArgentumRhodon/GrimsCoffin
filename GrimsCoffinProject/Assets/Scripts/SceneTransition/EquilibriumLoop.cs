using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquilibriumLoop : MonoBehaviour
{
    [SerializeField] private GameObject spawnPos;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControllerForces>() != null)
           PlayerControllerForces.Instance.transform.position = new Vector3(spawnPos.transform.position.x, PlayerControllerForces.Instance.transform.position.y, 0);
    }
}
