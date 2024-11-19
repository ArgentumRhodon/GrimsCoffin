using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Enemy enemyScript;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image fill;

    private float maxHP;

    // Start is called before the first frame update
    void Start()
    {
        enemyScript = this.GetComponentInParent<Enemy>();
        maxHP = enemyScript.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyScript.health < maxHP)
        {
            healthBar.SetActive(true);
        }

        fill.fillAmount = enemyScript.health / maxHP;
    }
}
