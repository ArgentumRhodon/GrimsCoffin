using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private string bossName = "???";
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI nameText;

    public DenialBoss bossScript;
    public float maxHP = 300;

    // Start is called before the first frame update
    void Start()
    {
        //bossScript = GameObject.Find(bossName).GetComponent<DenialBoss>();
        //maxHP = bossScript.health;
        DOVirtual.DelayedCall(.1f, SetReferences, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (bossScript.health <= 0)
            Destroy(this.gameObject);

        healthFill.fillAmount = Mathf.MoveTowards(healthFill.fillAmount, (bossScript.health / maxHP), Time.deltaTime);
    }

    private void SetReferences()
    {
        nameText.text = bossName;
        UIManager.Instance.bossHealthBar = this.gameObject;
    }
}
