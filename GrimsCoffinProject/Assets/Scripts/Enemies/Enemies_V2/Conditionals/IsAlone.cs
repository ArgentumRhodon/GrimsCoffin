using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class IsAlone : EnemyConditional
    { 
        public override TaskStatus OnUpdate()
        {
            if(enemyScript.CombatCoordinator != null)
            {
                if(enemyScript.CombatCoordinator.EnemiesInCombat.Count == 1 && enemyScript.CombatCoordinator.EnemiesInCombat.ContainsKey(enemyScript) && enemyScript.CombatCoordinator.MaxTicketTotal == 0)
                {
                    return TaskStatus.Success;
                }
            }
            return TaskStatus.Failure;
        }
    }
}
