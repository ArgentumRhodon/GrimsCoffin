using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : Enemy
{
    [SerializeField] public Collider2D roomBounds;
    [SerializeField] protected GameObject healthBar;

    private CinemachineConfiner followCameraConfiner;
    private Collider2D bossCameraConfiner;
    private Collider2D mainCameraConfiner;

    protected override void Start()
    {
        base.Start();
        roomBounds = GameObject.Find("RoomBounds").GetComponent<Collider2D>();
        UIManager.Instance.bossHealthBar.GetComponent<BossHealthBar>().SetupHealthBar();
        UIManager.Instance.bossHealthBar.SetActive(true);
    }

    private void CreateHealthBar()
    {
        GameObject healthbar = Instantiate(healthBar, UIManager.Instance.gameUI.transform);
        healthbar.GetComponent<BossHealthBar>().bossScript = this;
        healthBar.GetComponent<BossHealthBar>().maxHP = health;
    }

    public override void DestroyEnemyGO()
    {
        if (PlayerControllerForces.Instance.currentHP > 0)
        {
            UIManager.Instance.endStateText.SetActive(true);
            UIManager.Instance.bossMapIcon.SetActive(false);
        }

        //Instantiate(creditsPrefab);

        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }
}

