using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    //Enemy to spawn at this location
    [SerializeField] private Enemy enemyToSpawn;

    void Start()
    {
        
    }

    /// <summary>
    /// Spawn the enemy within the associated room
    /// </summary>
    /// <param name="room">The room to spawn the enemy in, used by the EnemyManager script</param>
    /// <returns></returns>
    public Enemy SpawnEnemy(GameObject room)
    {
        //Instatiates the enemy at the position of the spawn point with the room as the enemy's parent
        Instantiate(enemyToSpawn.gameObject, this.transform.position, Quaternion.identity, room.transform);
        return enemyToSpawn;
    }
}
