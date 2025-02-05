using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsDead : EnemyConditional
    {
        public string animationTriggerName;
        public override TaskStatus OnUpdate()
        {
            if (enemyScript.health < 0)
                animator.SetTrigger(animationTriggerName);

            return enemyScript.health < 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
