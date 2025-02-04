using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Idle : EnemyAction
    {
        public string animationTriggerName = "Idle";

        // Start is called before the first frame update
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
