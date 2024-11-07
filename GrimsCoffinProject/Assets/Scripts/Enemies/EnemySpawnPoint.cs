using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Enemy enemyToSpawn;

    void Start()
    {
        
    }

    public Enemy SpawnEnemy(GameObject room)
    {
        Instantiate(enemyToSpawn.gameObject, this.transform.position, Quaternion.identity, room.transform);
        return enemyToSpawn;
    }
}
