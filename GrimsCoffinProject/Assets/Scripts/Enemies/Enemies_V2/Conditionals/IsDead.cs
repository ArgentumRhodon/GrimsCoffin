using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsDead : EnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            return enemyScript.health <= 0 ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            if(enemyScript.health <= 0)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Corpse"))
                {
                    animator.Play("Dead");
                    //animator.SetTrigger("Dead");
                }
            }
        }
    }
}
