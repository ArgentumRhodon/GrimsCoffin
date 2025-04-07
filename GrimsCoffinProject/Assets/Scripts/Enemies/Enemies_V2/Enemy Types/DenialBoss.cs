using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DenialBoss : Enemy
{
    public enum Attacks
    {
        AOE,
        Laser,
        Blast
    }

    [SerializeField] public Collider2D[] AOEColliders;
    [SerializeField] public Collider2D laserCollider;
    [SerializeField] public Collider2D roomBounds;
    [SerializeField] private GameObject healthBar;

    private CinemachineConfiner followCameraConfiner;
    private Collider2D bossCameraConfiner;
    private Collider2D mainCameraConfiner;

    protected override void Start()
    {
        base.Start();
        roomBounds = GameObject.Find("RoomBounds").GetComponent<Collider2D>();
        UIManager.Instance.bossHealthBar.GetComponent<BossHealthBar>().SetupHealthBar();
        UIManager.Instance.bossHealthBar.SetActive(true);

/*        //Follow cam confiner
        followCameraConfiner = GameObject.Find("FollowCam").GetComponent<CinemachineConfiner>();

        //Sets to most recent camera confiner and updates to the main confiner
        mainCameraConfiner = followCameraConfiner.m_BoundingShape2D;
        bossCameraConfiner = GameObject.Find("DenialBossCamera").GetComponent<PolygonCollider2D>();

        followCameraConfiner.m_BoundingShape2D = bossCameraConfiner;*/
    }

    private void CreateHealthBar()
    {
        GameObject healthbar = Instantiate(healthBar, UIManager.Instance.gameUI.transform);
        healthbar.GetComponent<BossHealthBar>().bossScript = this;
        healthBar.GetComponent<BossHealthBar>().maxHP = health;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        foreach(Collider2D collider in AOEColliders)
        {
            CheckCollisionWithPlayer(collider, AttackDamage);
        }
        CheckCollisionWithPlayer(laserCollider, AttackDamage);
    }

    public override void DestroyEnemyGO()
    {
        if (PlayerControllerForces.Instance.currentHP > 0)
        {
            UIManager.Instance.endStateText.SetActive(true);
            UIManager.Instance.bossMapIcon.SetActive(false);
        }

        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }



/*    public void BackToMainCamera()
    {
        followCameraConfiner.m_BoundingShape2D = mainCameraConfiner;
    }*/
}
