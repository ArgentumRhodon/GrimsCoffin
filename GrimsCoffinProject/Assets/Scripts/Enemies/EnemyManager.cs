using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //List of spawn points within the room
    [SerializeField] private List<EnemySpawnPoint> enemySpawns;

    //List of active enemies within the room
    [SerializeField] private List<GameObject> activeEnemies;

    [SerializeField]
    private bool isArena;

   

    private void Start()
    {

    }

    //Spawns enemies inside of the room with this script
    //TODO: Tie this method to whenever the room is entered by the player
    public void SpawnEnemies()
    {
        foreach (EnemySpawnPoint spawn in enemySpawns)
        {
            //Spawn the enemy and add it to the list of active enemies
            if (spawn != null) 
                activeEnemies.Add(spawn.SpawnEnemy(this.gameObject));
        }
    }

    //Deletes all the enemies within the room
    //TODO: Tie this method to whenever the player exits the room
    public void DeleteEnemies()
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].GetComponent<Enemy>().DestroyEnemy();
            i--;
        }
    }

    //Remove an enemy from the list of active enemies
    //Used within the Enemy script whenever they are destroyed
    public void RemoveActiveEnemy(GameObject enemyToRemove)
    {
        activeEnemies.Remove(enemyToRemove);
        //Debug.Log("Removed");
        //Debug.Log(activeEnemies);
        if(isArena && activeEnemies.Count == 0)
        {
            this.GetComponent<ArenaManager>().CombatEnd();
        }
    }
}
