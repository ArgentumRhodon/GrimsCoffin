using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeFinisherState : MeleeBaseState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackIndex = 4;
        duration = 0.2f;
        animator.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (fixedtime >= duration)
        {
            stateMachine.SetNextStateToMain();
        }
    }
}
