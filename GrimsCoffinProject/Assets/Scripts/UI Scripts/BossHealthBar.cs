using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Pathfinding.Ionic.Zip;


public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private string bossName = "DenialBoss";
    [SerializeField] private string bossDisplayName = "???";
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI nameText;

    public Enemy bossScript;
    public float maxHP = 300;

    // Start is called before the first frame update
    void Start()
    {
        
        if (GameObject.Find(bossName) == null)
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bossScript != null)
        {
            if (bossScript.health <= 0)
                Destroy(this.gameObject);

            healthFill.fillAmount = Mathf.MoveTowards(healthFill.fillAmount, (bossScript.health / maxHP), Time.deltaTime);
        }
    }

    public void SetupHealthBar()
    {
        bossScript = GameObject.Find(bossName + "(Clone)").GetComponent<Enemy>();
        maxHP = bossScript.health;
        nameText.text = bossDisplayName;
    }
}
