using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class BasicAttack : EnemyAction
    {
        public string animationTriggerName;

        public override void OnStart()
        {
            //animator.SetTrigger(animationTriggerName);
            animator.Play("BasicSkeleton_Attack2");
            enemyScript.enemyStateList.IsAttacking = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BasicSkeleton_Attack2"))
            {
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            enemyScript.enemyStateList.IsAttacking = false;
        }
    }
}
