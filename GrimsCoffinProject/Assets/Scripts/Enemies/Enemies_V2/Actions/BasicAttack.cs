using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class BasicAttack : EnemyAction
    {
        public string animationTriggerName;
        public bool startedAttack;

        public override void OnStart()
        {           
            animator.Play(animationTriggerName);
            //Debug.Log(animationTriggerName);
            startedAttack = true;
            enemyScript.enemyStateList.IsAttacking = true;
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).ToString());
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationTriggerName) && startedAttack)
            {
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Running;
        }

        public override void OnEnd()
        {
            enemyScript.enemyStateList.IsAttacking = false;
        }
    }
}
