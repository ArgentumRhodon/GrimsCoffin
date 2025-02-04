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
            TurnAround();
        }

        public override TaskStatus OnUpdate()
        {
            if (hasTurned)
            {
/*                enemyScript.airChecker.IsColliding = true;
                enemyScript.wallChecker.IsColliding = false;*/
                return TaskStatus.Success;
            }             
            else
                return TaskStatus.Running;
        }

        private void TurnAround()
        {
            //Transform local scale of object
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            enemyScript.IsFacingRight = !enemyScript.IsFacingRight;
            hasTurned = true;
            enemyScript.Direction *= -1;

            //Updates scale of UI so that it is always facing right
            Vector3 tempScale = enemyCanvas.transform.localScale;
            tempScale.x *= -1;
            enemyCanvas.transform.localScale = tempScale;
        }
    }
}
