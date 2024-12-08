using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject spiritPrefab;
    [SerializeField] private bool inEquilibrium;
    [SerializeField] int spiritID;

    private Spirit spiritToSpawn;


    private void Start()
    {
        spiritToSpawn = spiritPrefab.GetComponentInChildren<Spirit>();
        spiritToSpawn.spiritID = (Spirit.SpiritName)spiritID;
        SpawnSpirit();
    }

    public void SpawnSpirit()
    {
        //Spawn Spirit in Equilibrium if it's collected, Spawn it in the Drift if it isn't
        if ((SpiritCollected() && inEquilibrium) || (!SpiritCollected() && !inEquilibrium))
        {
            Debug.Log("Spawning Spirit");
            GameObject spirit = Instantiate(spiritPrefab, this.transform.position, Quaternion.identity);
            spirit.GetComponentInChildren<Spirit>().spiritID = (Spirit.SpiritName)spiritID;
        }

    }

    private bool SpiritCollected()
    {
        //Debug.Log(spiritToSpawn.spiritID.ToString());
        if (PlayerPrefs.GetInt(spiritToSpawn.spiritID.ToString()) == 1)
        {
            Debug.Log("Spirit Collected");
            return true;
        }
        
        else
            return false;
    }
}
