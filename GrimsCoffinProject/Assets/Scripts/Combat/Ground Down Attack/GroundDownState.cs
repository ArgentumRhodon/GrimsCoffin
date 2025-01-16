using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDownState : MeleeBaseState
{
    //Impact force to 
    private Vector2 impactPoint;

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 2; //Not a combo so may not need this
        attackDamage = playerCombat.Data.groundDownDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.gDownAttackDuration;

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

        //Reset state after timer
        if (playerCombat.AttackDurationTime < 0)
        {
            stateMachine.SetNextStateToMain();
        }
    }

    private void FindImpactPoint()
    {

    }

    protected override Vector2 KnockbackForce(Vector2 enemyPos)
    {
        //Check direction for knockback
        int direction;
        if (IsPlayerOnRight(enemyPos))
            direction = -1;
        else
            direction = 1;

        return new Vector2(direction, 0);
    }
}
