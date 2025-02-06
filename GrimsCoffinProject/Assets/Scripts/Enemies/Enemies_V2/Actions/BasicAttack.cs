using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class BasicAttack : EnemyAction
    {
        public string animationTriggerName;

        public override void OnStart()
        {
            animator.SetTrigger(animationTriggerName);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
