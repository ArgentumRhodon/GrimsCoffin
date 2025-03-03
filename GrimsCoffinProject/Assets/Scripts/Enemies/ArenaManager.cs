using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField]
    private GameObject arenaDoors;

    [SerializeField]
    private EnemyManager enemyMgr;


    [SerializeField]
    public int arenaIndex;
    public bool arenaCleared = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !arenaCleared)
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            enemyMgr.SpawnEnemies();
            arenaDoors.SetActive(true);
        }
    }

    public void CombatEnd()
    {
        enemyMgr.DeleteEnemies();
        arenaDoors.SetActive(false);
    }

    public void ClearArena()
    {
        CombatEnd();
        UIManager.Instance.ShowSaveIcon(2);
        PersistentDataManager.Instance.ClearArena(arenaIndex);
    }

    public void ResetArena()
    {
        this.GetComponent<BoxCollider2D>().enabled = true;
    }
}
