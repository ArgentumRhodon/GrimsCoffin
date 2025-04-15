using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsInRange : EnemyConditional
    {
        public float range;
        public bool CheckOutsideRange = false;

        public override TaskStatus OnUpdate()
        {
            if (!CheckOutsideRange && enemyScript.FindPlayerDistanceX() <= range)
            {
                return TaskStatus.Success;
            }
            else if(CheckOutsideRange && enemyScript.FindPlayerDistanceX() >= range)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}
