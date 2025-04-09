using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsStaggered : EnemyConditional
    {
        public string animationTriggerName;
        public override TaskStatus OnUpdate()
        {
            if (enemyScript.enemyStateList.IsStaggered)
            {
                //animator.Play(animationTriggerName);
                //animator.SetTrigger("Stagger");
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
