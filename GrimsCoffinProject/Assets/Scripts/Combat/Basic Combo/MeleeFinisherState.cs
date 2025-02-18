using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFinisherState : MeleeBaseState
{
    public MeleeFinisherState() : base()
    {
        attackIndex = 4;
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables
        attackDamage = playerCombat.Data.combo4Damage;
        playerCombat.AttackDurationTime = playerCombat.Data.comboAttackDuration;
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
