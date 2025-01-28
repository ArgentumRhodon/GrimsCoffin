using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDownState : MeleeBaseState
{
    //Impact force to 
    private Vector2 impactPoint;
    private  List<Collider2D> collidersAttached;

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 3;
        attackDamage = playerCombat.Data.aerialDownDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.aDownAttackDuration;

        animator.SetTrigger("Attack");
        animator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_T.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_B.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_T.SetTrigger("Attack");
        playerAnimator_B.SetTrigger("Attack");
    }

    //Continue downwards attack until player is on ground
    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        //If player is on the ground, end it
        if (PlayerControllerForces.Instance.Grounded() && playerCombat.AttackDurationTime < 0)
        {
            stateMachine.SetNextStateToMain();
            playerCombat.IsAerialAttacking = false;
        }
    }

    private void FindImpactPoint()
    {

    }

/*    protected virtual void RegisterAttack(Collider2D collidersToDamage)
    {
        Vector2 knockbackForce = KnockbackForce(collidersToDamage.gameObject.GetComponent<Enemy>().transform.position);
        collidersToDamage.gameObject.GetComponent<Enemy>().TakeDamage(knockbackForce, attackDamage);
        collidersDamaged.Add(collidersToDamage);
    }*/

    /*    protected override Vector2 KnockbackForce(Vector2 enemyPos)
        {
            //Check direction for knockback
            int direction;
            if (IsPlayerOnRight(enemyPos))
                direction = -1;
            else
                direction = 1;

            return new Vector2(direction, 0);
        }*/
}
