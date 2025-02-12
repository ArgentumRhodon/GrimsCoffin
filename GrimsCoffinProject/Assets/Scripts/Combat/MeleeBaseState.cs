using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MeleeBaseState : CState
{
    //Variables
    [SerializeField] protected float duration;
    protected Animator animator;

    // Player Animation Stuff
    protected Animator playerAnimator; // Top

    //Bool to check if it should continue the combo or not
    protected bool shouldCombo; 
    //Index of sequence in attack
    protected int attackIndex;
    protected float attackDamage;

    protected PlayerCombat playerCombat;

    //Cached hit collider component of this attack
    protected Collider2D hitCollider;
    //Cached already struck objects of said attack to avoid overlapping attacks on same target
    protected List<Collider2D> collidersDamaged;


    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        playerCombat = _stateMachine.GetComponent<PlayerCombat>();
        animator = playerCombat.scytheAnimator;
        playerAnimator = playerCombat.animator;
        collidersDamaged = new List<Collider2D>();
        hitCollider = playerCombat.hitbox;

        // First combo state has index of 1
        if(attackIndex > 0)
        {
            PlayerAnimationManager.Instance.ChangeAnimationState(
                PlayerAnimationStates.GetComboAnimation(attackIndex)
            );
        }
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

        //Debug.Log("Attack is running");

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    if (collidersToDamage[i].GetComponent<PolygonCollider2D>() != null)
                        continue;

                    RegisterAttack(collidersToDamage[i]);
                    //RegisterAttack(hitTeamComponent.gameObject.GetComponent<Collider2D>());
                }
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.BreakWall)
                {
                    if (collidersToDamage[i].GetComponent<PolygonCollider2D>() != null)
                        continue;

                    RegisterAttackWall(collidersToDamage[i]);
                }
            }
        }
    }

    protected virtual void RegisterAttack(Collider2D collidersToDamage)
    {
        Vector2 knockbackForce = KnockbackForce(collidersToDamage.gameObject.GetComponent<Enemy>().transform.position);
        collidersToDamage.gameObject.GetComponent<Enemy>().TakeDamage(knockbackForce, attackDamage);
        collidersDamaged.Add(collidersToDamage);
    }

    protected virtual void RegisterAttackWall(Collider2D collidersToDamage)
    {
        collidersToDamage.gameObject.GetComponent<BreakableWall>().TakeDamage(attackDamage);
        collidersDamaged.Add(collidersToDamage);
    }

    protected virtual Vector2 KnockbackForce(Vector2 enemyPos)
    {
        //Check direction for knockback
        int direction;
        if (IsPlayerOnRight(enemyPos))
            direction = -1;
        else
            direction = 1;

        return new Vector2(direction, 0);
    }

    protected bool IsPlayerOnRight(Vector2 enemyPos)
    {
        return playerCombat.gameObject.transform.position.x > enemyPos.x;
    }
}
