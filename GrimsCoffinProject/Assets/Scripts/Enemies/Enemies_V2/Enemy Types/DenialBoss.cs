using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DenialBoss : Boss
{
    public enum Attacks
    {
        AOE,
        Laser,
        Blast
    }

    [SerializeField] public Collider2D[] AOEColliders;
    [SerializeField] public Collider2D laserCollider;

    [SerializeField] private GameObject creditsPrefab;

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

        Instantiate(creditsPrefab);

        this.gameObject.GetComponentInParent<EnemyManager>().RemoveActiveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }



/*    public void BackToMainCamera()
    {
        followCameraConfiner.m_BoundingShape2D = mainCameraConfiner;
    }*/
}
