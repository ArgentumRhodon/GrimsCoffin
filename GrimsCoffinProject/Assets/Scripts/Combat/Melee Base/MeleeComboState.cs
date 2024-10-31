using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComboState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 2;
        duration = 0.5f;
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (fixedtime >= duration)
        {
            if (shouldCombo && _stateMachine.RegisteredAttack)
            {
                stateMachine.SetNextState(new MeleeFinisherState());
                _stateMachine.RegisteredAttack = false;
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
