using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntryState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 1;
        duration = 0.2f;
        animator.SetTrigger("Attack" + attackIndex);
        //Debug.Log("Attack" + attackIndex);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        //Check timing to see if the player should combo or not
        if (fixedtime >= duration)
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
