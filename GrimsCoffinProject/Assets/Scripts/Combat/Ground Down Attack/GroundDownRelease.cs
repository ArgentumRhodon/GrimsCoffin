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
        attackDamage = playerCombat.Data.groundDownDamage;
        playerCombat.AttackDurationTime = playerCombat.Data.gDownAttackDuration;

        PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.GroundDown);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);

        if (playerCombat.ShouldResetCombo())
        {
            PlayerControllerForces playerController = playerCombat.GetComponent<PlayerControllerForces>();
            playerController.WalkModifier = 1;

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

        return new Vector2(direction * playerCombat.Data.groundDownwardEForce.x, playerCombat.Data.groundDownwardEForce.y);
    }

    protected override void RegisterAttack(Collider2D collidersToDamage)
    {
        Vector2 knockbackForce = KnockbackForce(collidersToDamage.gameObject.GetComponent<Enemy>().transform.position);
        collidersToDamage.gameObject.GetComponent<Enemy>().TakeDamage(knockbackForce, attackDamage, true, .5f);
        collidersDamaged.Add(collidersToDamage);
    }
}
