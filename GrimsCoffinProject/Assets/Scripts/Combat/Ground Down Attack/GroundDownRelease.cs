using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDownRelease : MeleeBaseState
{
    public GroundDownRelease() : base()
    {
        attackIndex = -1;
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Set attack variables and animation
        attackDamage = PlayerControllerForces.Instance.Data.groundDownDamage;
        playerCombat.AttackDurationTime = PlayerControllerForces.Instance.Data.gDownAttackDuration;

        PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.AttackDownExecute);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (playerCombat.ShouldResetCombo())
        {
            //PlayerControllerForces playerController = playerCombat.GetComponent<PlayerControllerForces>();
            //playerController.WalkModifier = 1;

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

        return new Vector2(direction * PlayerControllerForces.Instance.Data.groundDownwardEForce.x, PlayerControllerForces.Instance.Data.groundDownwardEForce.y);
    }

    protected override void RegisterAttack(Collider2D collidersToDamage)
    {
        Vector2 knockbackForce = KnockbackForce(collidersToDamage.gameObject.GetComponent<Enemy>().transform.position);
        collidersToDamage.gameObject.GetComponent<Enemy>().TakeDamage(knockbackForce, attackDamage * PlayerControllerForces.Instance.Data.damageMultiplier, true, .5f, .08f);
        collidersDamaged.Add(collidersToDamage);
    }
}
