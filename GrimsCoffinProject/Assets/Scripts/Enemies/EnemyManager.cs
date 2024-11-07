using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnPoint> enemySpawns;
    [SerializeField] private List<Enemy> activeEnemies;

    private void Start()
    {
        SpawnEnemies();
    }


    public void SpawnEnemies()
    {
        foreach (EnemySpawnPoint spawn in enemySpawns)
        {
            
            activeEnemies.Add(spawn.SpawnEnemy(this.gameObject));
        }
    }

    public void DeleteEnemies()
    {
        foreach (Enemy enemy in activeEnemies)
        {
            enemy.DestroyEnemy();
        }
    }

    public void RemoveActiveEnemy(Enemy enemyToRemove)
    {
        activeEnemies.Remove(enemyToRemove);
    }
}
