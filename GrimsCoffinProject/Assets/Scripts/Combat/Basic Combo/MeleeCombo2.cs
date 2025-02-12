using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCombo2 : MeleeBaseState
{
    public MeleeCombo2() : base()
    {
        attackIndex = 3;
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables
        attackDamage = playerCombat.Data.combo3Damage;
        playerCombat.AttackDurationTime = playerCombat.Data.comboAttackDuration;
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
