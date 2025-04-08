using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Playables;

namespace Core.AI
{
    public class TurnToPlayer : EnemyAction
    {
        public override void OnStart()
        {
            enemyScript.TurnToPlayer();
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

    }
}
