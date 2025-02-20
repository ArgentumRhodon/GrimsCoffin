using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;

namespace Core.AI
{
    public class ForwardAttack : EnemyAction
    {
        public string animationTriggerName;

        public float attackDelay = 0;
        public float forwardDelay = 0;
        public float attackDuration = 0;
        public int attackDamageIndex;

        public float forwardForce;

        private bool attackCompleted;

        public override void OnStart()
        {
            attackCompleted = false;

            DOVirtual.DelayedCall(attackDelay, Attack, false);
        }

        public override TaskStatus OnUpdate()
        {
            if (attackCompleted)
            {
                enemyScript.enemyStateList.IsAttacking = false;
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Running;
        }

        private void Attack()
        {
            //Attack
            enemyScript.AttackDamage = enemyScript.attackDamages[attackDamageIndex];
            animator.Play(animationTriggerName);
            enemyScript.enemyStateList.IsAttacking = true;

            DOVirtual.DelayedCall(forwardDelay, MoveForward, false);

            //Delayed call for attack duration
            DOVirtual.DelayedCall(attackDuration, FinishAttack, false);
        }

        private void FinishAttack()
        {
            attackCompleted = true;
        }

        private void MoveForward()
        {
            //Forward force
            int direction = enemyScript.enemyStateList.IsFacingRight ? 1 : -1;
            rb.AddForce(Vector2.right * direction * forwardForce, ForceMode2D.Impulse);
        }
    }
}
