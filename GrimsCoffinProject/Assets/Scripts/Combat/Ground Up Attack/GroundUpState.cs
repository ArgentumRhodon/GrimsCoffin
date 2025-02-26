using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundUpState : MeleeBaseState
{
    public GroundUpState() : base() {
        attackIndex = 3;
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackDamage = playerCombat.Data.groundUpDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.gUpAttackDuration;
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (playerCombat.AttackDurationTime < 0)
        {
            stateMachine.SetNextStateToMain();
        }
    }

    protected override Vector2 KnockbackForce(Vector2 enemyPos)
    {
        //Check direction for knockback
        int direction;
        if (IsPlayerOnRight(enemyPos))
            direction = -1;
        else
            direction = 1;

        return new Vector2(direction * playerCombat.Data.groundUpwardEForce.x, playerCombat.Data.groundUpwardEForce.y);
    }
}
