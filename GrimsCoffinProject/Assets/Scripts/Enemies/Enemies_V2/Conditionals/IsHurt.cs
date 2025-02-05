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
            if(enemyScript.IsDamaged)
            {
                animator.SetTrigger(animationTriggerName);
            }
                

            return enemyScript.IsDamaged ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
