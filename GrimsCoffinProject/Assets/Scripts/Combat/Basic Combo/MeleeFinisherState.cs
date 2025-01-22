using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFinisherState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 3;
        attackDamage = 10;
        playerCombat.AttackDurationTime = .35f;

        //Animations
        animator.SetTrigger("Attack");
        animator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator.SetTrigger("Attack");
        //Debug.Log("Player Attack " + attackIndex);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (playerCombat.ShouldResetCombo())
        {
            stateMachine.SetNextStateToMain();        
        }
    }
}
