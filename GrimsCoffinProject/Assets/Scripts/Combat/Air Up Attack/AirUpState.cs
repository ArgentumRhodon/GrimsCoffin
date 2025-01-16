using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirUpState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 2; //Not a combo so may not need this
        attackDamage = playerCombat.Data.aerialUpDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.aUpAttackDuration;

/*        animator.SetTrigger("Attack");
        animator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_T.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_B.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_T.SetTrigger("Attack");
        playerAnimator_B.SetTrigger("Attack");*/
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
