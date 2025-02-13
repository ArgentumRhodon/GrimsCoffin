using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Idle : EnemyAction
    {
        public string animationTriggerName;

        // Start is called before the first frame update
        public override void OnStart()
        {
            //animator.SetTrigger(animationTriggerName);
            animator.Play(animationTriggerName);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
