using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject spiritPrefab;
    [SerializeField] private bool inEquilibrium;

    private Spirit spiritToSpawn;

    private void Start()
    {
        spiritToSpawn = spiritPrefab.GetComponentInChildren<Spirit>();
    }

    public void SpawnSpirit()
    {
        //Spawn Spirit in Equilibrium if it's collected, Spawn it in the Drift if it isn't
        if ((SpiritCollected() && inEquilibrium) || (!SpiritCollected() && !inEquilibrium))
        {
            Debug.Log("Spawning Spirit");
            Instantiate(spiritPrefab, this.transform.position, Quaternion.identity);
        }

    }

    private bool SpiritCollected()
    {
        if (PlayerPrefs.GetInt(spiritToSpawn.spiritID.ToString()) == 1)
        {
            Debug.Log("Spirit Collected");
            return true;
        }
        
        else
            return false;
    }
}
