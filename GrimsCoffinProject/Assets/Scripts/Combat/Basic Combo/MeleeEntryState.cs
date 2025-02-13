using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : MeleeBaseState
{
    public MeleeEntryState() : base()
    {
        attackIndex = 1;
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables
        attackDamage = playerCombat.Data.combo1Damage;
        playerCombat.AttackDurationTime = playerCombat.Data.comboAttackDuration;
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (_stateMachine.RegisteredAttack)
        {
            stateMachine.SetNextState(new MeleeComboState());
            _stateMachine.RegisteredAttack = false;
        }
        else if(playerCombat.ShouldResetCombo())
        {
            stateMachine.SetNextStateToMain();
        }
        
    }
}
