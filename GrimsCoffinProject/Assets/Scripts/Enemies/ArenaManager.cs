using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    [SerializeField]
    private GameObject arenaDoors;

    [SerializeField]
    private EnemyManager enemyMgr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            enemyMgr.SpawnEnemies();
            arenaDoors.SetActive(true);
        }
    }

    public void CombatEnd()
    {
        arenaDoors.SetActive(false);
    }
}
