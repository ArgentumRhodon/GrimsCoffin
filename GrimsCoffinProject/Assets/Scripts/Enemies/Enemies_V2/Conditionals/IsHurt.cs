using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsHurt : EnemyConditional
    {
        public string animationTriggerName;
        public override TaskStatus OnUpdate()
        {
            if(enemyScript.IsDamaged && !enemyScript.enemyStateList.IsAttacking)
            {
                animator.Play(animationTriggerName);
            }
                

            return enemyScript.IsDamaged ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
