using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquilibriumSpirits : MonoBehaviour
{
    [SerializeField] List<SpiritSpawnPoint> spiritSpawns;


    void Start()
    {
        SpawnAllCollectedSpirits();
    }

    private void SpawnAllCollectedSpirits()
    {
        foreach (SpiritSpawnPoint spawn in spiritSpawns)
        {
            spawn.SpawnSpirit();
        }
    }
}
