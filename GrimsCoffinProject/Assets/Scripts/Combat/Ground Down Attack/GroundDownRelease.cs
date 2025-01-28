using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDownRelease : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackDamage = playerCombat.Data.groundDownDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.gDownAttackDuration; 

        //Animations
        animator.SetTrigger("GroundDown");
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
            PlayerControllerForces playerController = playerCombat.GetComponent<PlayerControllerForces>();
            playerController.WalkModifier = 1;

            stateMachine.SetNextStateToMain();
        }
    }
}
