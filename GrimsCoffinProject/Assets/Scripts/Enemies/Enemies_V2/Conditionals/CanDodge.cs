using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class CanDodge : EnemyConditional
    {
        [Range(1, 10)] public int chance;
        public override TaskStatus OnUpdate()
        {
            return (enemyScript.HurtInSuccessionTotal >= enemyScript.HurtDodgeMin
                && !enemyScript.enemyStateList.IsAttacking && Random.Range(0, 10) < chance) ? TaskStatus.Success : TaskStatus.Failure;
        }
        
        public override void OnEnd()
        {
            if (enemyScript.HurtInSuccessionTotal >= enemyScript.HurtDodgeMin
                && !enemyScript.enemyStateList.IsAttacking)
            {
                enemyScript.HurtSuccessionTimer = 0;
                enemyScript.HurtInSuccessionTotal = 0;
            }
        }
    }
}
