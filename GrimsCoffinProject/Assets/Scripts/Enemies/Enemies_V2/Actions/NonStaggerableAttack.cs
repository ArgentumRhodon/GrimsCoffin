using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Core.AI
{
    public class NonStaggerableAttack : EnemyAction
    {
        public string animationTriggerName;
        public string animationIdleName;

        public float attackDelay = 0;
        public float attackDuration = 0;
        public int attackDamageIndex;

        private bool attackCompleted;

        private bool originalStaggerState;
        private bool originalKnockbackState;   
        private bool originalGetHitCanceled;

        public override void OnStart()
        {
            //Attack not completed yet
            attackCompleted = false;

            //Save original states and make it non-staggerable
            originalStaggerState = enemyScript.CanBeStaggered;
            enemyScript.CanBeStaggered = false;

            originalKnockbackState = enemyScript.CanTakeKnockback;
            enemyScript.CanTakeKnockback = false;

            originalGetHitCanceled = enemyScript.GetsHitCanceled;
            enemyScript.GetsHitCanceled = false;

            //Color? - Might be implemented later, but suited better with an animation of some sort
            enemyScript.SpriteRenderer.color = new Color(.95f,.75f,.75f);

            //Make sure enemy is not already staggered
            if (!enemyScript.enemyStateList.IsStaggered)
                DOVirtual.DelayedCall(attackDelay, Attack, false);
        }

        public override TaskStatus OnUpdate()
        {
            if (attackCompleted)
            {
                enemyScript.enemyStateList.IsAttacking = false;
                return TaskStatus.Success;
            }
            else if (enemyScript.enemyStateList.IsStaggered)
                return TaskStatus.Failure;
            else
                return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            //Use ticket if it has one, no matter what it should remove ticket if it attacks or not
            enemyScript.HasAttackTicket = false;
            enemyScript.CombatCoordinator.UseAttack(enemyScript);

            //Return state back to normal
            enemyScript.CanBeStaggered = originalStaggerState;
            enemyScript.CanTakeKnockback = originalKnockbackState;
            enemyScript.GetsHitCanceled = originalGetHitCanceled;
            enemyScript.SpriteRenderer.color = Color.white;
        }

        private void Attack()
        {
            //Attack animation
            enemyScript.AttackDamage = enemyScript.attackDamages[attackDamageIndex];
            animator.Play(animationTriggerName);
            enemyScript.enemyStateList.IsAttacking = true;

            //Update state for after the attack duration is done
            DOVirtual.DelayedCall(attackDuration, FinishAttack, false);
        }

        //Used to finish attack based off a delayed call
        private void FinishAttack()
        {
            attackCompleted = true;
        }
    }
}
