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
            if(enemyScript.IsDamaged && !enemyScript.enemyStateList.IsAttacking && enemyScript.CanBeStopped)// && enemyScript.IsStaggered)
            {
                animator.Play(animationTriggerName);
                return TaskStatus.Success;
            }
            
            return TaskStatus.Failure;
        }
    }
}
