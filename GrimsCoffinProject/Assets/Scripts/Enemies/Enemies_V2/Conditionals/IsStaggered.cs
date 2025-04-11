using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsStaggered : EnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            if (enemyScript.enemyStateList.IsStaggered)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
