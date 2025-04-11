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
        public string animationIdleName;

        public float attackDelay = 0;
        public float attackDuration = 0;
        public int attackDamageIndex;

        private bool attackCompleted;

        public override void OnStart()
        {
            attackCompleted = false;
            
            if(!enemyScript.enemyStateList.IsStaggered)
                DOVirtual.DelayedCall(attackDelay, Attack, false);
        }

        public override TaskStatus OnUpdate()
        {
            if (attackCompleted)
            {
                enemyScript.enemyStateList.IsAttacking = false;
                if (enemyScript.HasAttackTicket)
                {
                    //Should release ticket no matter what after ending their attack
                    //enemyScript.CombatCoordinator.UseAttack(enemyScript);
                    //enemyScript.HasAttackTicket = false;
                    animator.Play(animationIdleName);
                }
                return TaskStatus.Success;
            }
            else if(enemyScript.enemyStateList.IsStaggered)
                return TaskStatus.Failure;
            else
                return TaskStatus.Running;           
        }

        public override void OnEnd()
        {
            enemyScript.HasAttackTicket = false;
            enemyScript.CombatCoordinator.UseAttack(enemyScript);
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
