using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsClose : EnemyConditional
    {
        public bool isSuccessIfClose = true;
        public float range;

        public override TaskStatus OnUpdate()
        {
            if ((isSuccessIfClose && enemyScript.FindPlayerDistanceX() < range)
                || (!isSuccessIfClose && enemyScript.FindPlayerDistanceX() > range))
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
