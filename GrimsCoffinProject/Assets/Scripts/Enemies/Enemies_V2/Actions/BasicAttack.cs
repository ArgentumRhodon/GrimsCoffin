using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;

namespace Core.AI
{
    public class BasicAttack : EnemyAction
    {
        public string animationTriggerName;

        public float attackDelay = 0;
        public float attackDuration = 0;
        public int attackDamageIndex;

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
            enemyScript.AttackDamage = enemyScript.attackDamages[attackDamageIndex];
            animator.Play(animationTriggerName);
            enemyScript.enemyStateList.IsAttacking = true;


            DOVirtual.DelayedCall(attackDuration, FinishAttack, false);

        }

        private void FinishAttack()
        {
            attackCompleted = true;
        }
    }
}
