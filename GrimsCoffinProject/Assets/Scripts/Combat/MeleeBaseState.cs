using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : CState
{
    //Variables
    [SerializeField] protected float duration;
    protected Animator animator;

    // Player Animation Stuff
    protected Animator playerAnimator_T; // Top
    protected Animator playerAnimator_B; // Bottom

    //Bool to check if it should continue the combo or not
    protected bool shouldCombo; 
    //Index of sequence in attack
    protected int attackIndex;

    protected PlayerCombat playerCombat;


    //Cached hit collider component of this attack
    protected Collider2D hitCollider;
    //Cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider2D> collidersDamaged;


    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        playerCombat = _stateMachine.GetComponent<PlayerCombat>();
        animator = playerCombat.scytheAnimator;
        playerAnimator_T = playerCombat.animator_T;
        playerAnimator_B = playerCombat.animator_B;
        collidersDamaged = new List<Collider2D>();
        hitCollider = playerCombat.hitbox;
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (playerCombat.AttackDurationTime > 0f)
        {
            //Check for collisions
            Attack();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    protected void Attack()
    {
        //Attack the enemy, check for collisions
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    collidersToDamage[i].gameObject.GetComponent<Enemy>().TakeDamage();
                    //Debug.Log("Enemy Has Taken: " + attackIndex + " Damage");
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }

    }
}
