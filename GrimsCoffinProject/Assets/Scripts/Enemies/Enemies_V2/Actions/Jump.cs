using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.AI
{
    public class Jump : EnemyAction
    {
        public float horizontalForce = 5f;
        public float jumpForce = 10f;

        public float jumpTime;

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
