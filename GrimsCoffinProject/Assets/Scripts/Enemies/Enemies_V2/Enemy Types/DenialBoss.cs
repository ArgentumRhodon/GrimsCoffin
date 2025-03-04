using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        foreach(Collider2D collider in AOEColliders)
        {
            CheckCollisionWithPlayer(collider, AttackDamage);
        }
        CheckCollisionWithPlayer(laserCollider, AttackDamage);
    }
}
