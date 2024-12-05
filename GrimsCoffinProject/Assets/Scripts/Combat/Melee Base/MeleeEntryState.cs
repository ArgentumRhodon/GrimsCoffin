using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 0;
        duration = .35f;
        animator.SetTrigger("Attack");
        animator.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_T.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_B.SetFloat("comboRatio", attackIndex / 3f);
        playerAnimator_T.SetTrigger("Attack");
        playerAnimator_B.SetTrigger("Attack");
        Debug.Log("Attack" + attackIndex);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        //Check timing to see if the player should combo or not
        if (fixedtime >= duration)
        //if(playerCombat.LastAttackTime > 0)
        {
            if (shouldCombo && _stateMachine.RegisteredAttack)
            {
                stateMachine.SetNextState(new MeleeComboState());
                _stateMachine.RegisteredAttack = false;
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
