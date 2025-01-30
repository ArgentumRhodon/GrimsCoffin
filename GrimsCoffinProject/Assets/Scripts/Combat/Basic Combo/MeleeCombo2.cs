using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCombo2 : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 2;
        attackDamage = playerCombat.Data.combo3Damage;
        playerCombat.AttackDurationTime = playerCombat.Data.comboAttackDuration;

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

        if (_stateMachine.RegisteredAttack)
        {
            stateMachine.SetNextState(new MeleeFinisherState());
            _stateMachine.RegisteredAttack = false;
        }
        else if (playerCombat.ShouldResetCombo())
        {
            stateMachine.SetNextStateToMain();
        }
    }
}
