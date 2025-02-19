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
        public bool startedAttack;

        public float attackDelay = 0;
        public float attackDuration = 0;
        public int attackDamageIndex;

        private bool attackCompleted;

        public override void OnStart()
        {
            attackCompleted = false;
            //enemyScript.AttackDamage = attackDamage;

            DOVirtual.DelayedCall(attackDelay, Attack, false);
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log(enemyScript.attackDamages[attackDamageIndex]);
            enemyScript.CheckCollisionWithPlayer(enemyScript.attackCollider, enemyScript.attackDamages[attackDamageIndex]);
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
            animator.Play(animationTriggerName);
            enemyScript.enemyStateList.IsAttacking = true;

            DOVirtual.DelayedCall(attackDuration, FinishAttack, false);
        }

        private void FinishAttack()
        {
            attackCompleted = true;
        }

        public override void OnEnd()
        {

        }
    }
}
