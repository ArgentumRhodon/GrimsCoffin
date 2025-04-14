using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Playables;

namespace Core.AI
{
    public class Turn : EnemyAction
    {
        public override void OnStart()
        {
            if (enemyScript.enemyStateList.IsFacingRight)
                enemyScript.FaceRight(false);
            else
                enemyScript.FaceRight(true);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

    }
}
