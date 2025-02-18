using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Hurt : EnemyAction
    {
        // Start is called before the first frame update
        public override void OnStart()
        {
            enemyScript.IsDamaged = false;
            enemyScript.IsStaggered = false;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
