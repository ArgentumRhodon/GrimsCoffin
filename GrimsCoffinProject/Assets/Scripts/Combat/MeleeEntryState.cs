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
        duration = 0.5f;
        animator.SetTrigger("PlayerAttack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        //Check timing to see if the player should combo or not
        if (fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new MeleeComboState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
