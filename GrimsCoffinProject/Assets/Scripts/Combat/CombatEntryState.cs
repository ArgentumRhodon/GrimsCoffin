using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEntryState : CState
{
    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        CState nextState = (CState)new MeleeEntryState();
        stateMachine.SetNextState(nextState);
    }
}
