using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDownCharge : MeleeBaseState
{
    PlayerControllerForces playerController;

    public GroundDownCharge() : base()
    {
        attackIndex = -1; // Not part of combo
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.AttackDownReady);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        if (_stateMachine.RegisteredAttack)
        {
            stateMachine.SetNextState(new GroundDownRelease());
            _stateMachine.RegisteredAttack = false;
        }
        else if (!playerCombat.isHoldingDownOnGround)
        {
            _stateMachine.RegisteredAttack = false;
            stateMachine.SetNextStateToMain();
        }       
    }
}
