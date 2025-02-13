using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Playables;

namespace Core.AI
{
    public class Turn : EnemyAction
    {
        private bool hasTurned = false;
        private Canvas enemyCanvas;

        public string animationTriggerName;

        public override void OnStart()
        {
            enemyCanvas = gameObject.GetComponentInChildren<Canvas>();
            //animator.SetTrigger(animationTriggerName);
            //animator.Play("BasicSkeleton_Idle");

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
