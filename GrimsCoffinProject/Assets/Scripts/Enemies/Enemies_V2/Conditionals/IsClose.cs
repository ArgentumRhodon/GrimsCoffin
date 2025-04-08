using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsClose : EnemyConditional
    {
        private Collider2D collider;

        public override void OnStart()
        {
            collider = enemyScript.closeRangeCollider;
        }

        public override TaskStatus OnUpdate()
        {
            if (enemyScript.IsOverlapping(collider) && !enemyScript.enemyStateList.IsAttacking)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
