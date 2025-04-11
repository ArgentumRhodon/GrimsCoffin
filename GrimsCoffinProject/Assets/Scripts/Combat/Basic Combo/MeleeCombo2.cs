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
        attackDamage = PlayerControllerForces.Instance.Data.combo3Damage;
        playerCombat.AttackDurationTime = PlayerControllerForces.Instance.Data.comboAttackEndDuration;
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

    protected override Vector2 KnockbackForce(Vector2 enemyPos)
    {
        //Check direction for knockback
        int direction;
        if (IsPlayerOnRight(enemyPos))
            direction = -1;
        else
            direction = 1;

        return new Vector2(direction * PlayerControllerForces.Instance.Data.comboFinisherForce, 0);
    }

    protected override void RegisterAttack(Collider2D collidersToDamage)
    {
        Vector2 knockbackForce = KnockbackForce(collidersToDamage.gameObject.GetComponent<Enemy>().transform.position);
        collidersToDamage.gameObject.GetComponent<Enemy>().TakeDamage(knockbackForce, attackDamage, false, .1f, .05f);
        collidersDamaged.Add(collidersToDamage);
    }
}
