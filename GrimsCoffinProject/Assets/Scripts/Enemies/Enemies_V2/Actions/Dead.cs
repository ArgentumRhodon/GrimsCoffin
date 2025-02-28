using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Dead : EnemyAction
    {
        // Start is called before the first frame update
        public override void OnStart()
        {
            enemyScript.DestroyEnemyGO();
            enemyScript.behaviorTree.DisableBehavior();
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
