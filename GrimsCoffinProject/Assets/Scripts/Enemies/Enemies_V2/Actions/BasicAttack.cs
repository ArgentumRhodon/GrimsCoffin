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

        public override void OnStart()
        {
            DOVirtual.DelayedCall(attackDelay, Attack, false);
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).ToString());
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationTriggerName) && startedAttack)
            {
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Running;
        }

        private void Attack()
        {
            animator.Play(animationTriggerName);
            startedAttack = true;
            enemyScript.enemyStateList.IsAttacking = true;
        }

        public override void OnEnd()
        {
            enemyScript.enemyStateList.IsAttacking = false;
            startedAttack = false;
        }
    }
}
