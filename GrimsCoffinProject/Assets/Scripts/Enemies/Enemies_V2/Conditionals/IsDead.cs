using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsDead : EnemyConditional
    {
        public string animationTriggerName;
        public override TaskStatus OnUpdate()
        {
            return enemyScript.health <= 0 ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            if(enemyScript.health <= 0)
            {
                animator.Play(animationTriggerName);
                gameObject.GetComponent<TeamComponent>().teamIndex = TeamIndex.Neutral;
                //rb.gravityScale = 1;
                enemyScript.RemoveActiveEnemy();
            }
        }
    }
}
