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
    [SerializeField] public Collider2D LaserCollider;
    [SerializeField] public Collider2D RoomBounds;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        foreach(Collider2D collider in AOEColliders)
        {
            CheckCollisionWithPlayer(collider, AttackDamage);
        }
        CheckCollisionWithPlayer(LaserCollider, AttackDamage);
    }
}
