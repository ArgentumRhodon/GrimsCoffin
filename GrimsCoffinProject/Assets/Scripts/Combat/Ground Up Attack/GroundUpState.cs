using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundUpState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 2; //Not a combo so may not need this
        attackDamage = playerCombat.Data.groundUpDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.gUpAttackDuration;

        animator.SetTrigger("Attack");
        animator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator.SetTrigger("Attack");
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (playerCombat.AttackDurationTime < 0)
        {
            stateMachine.SetNextStateToMain();
        }
    }
}
