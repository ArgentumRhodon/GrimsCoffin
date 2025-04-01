using Core.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;

namespace Core.AI 
{
    public class CanSeePlayer : EnemyConditional
    {
        //Collider for the player's vision
        private Collider2D visionRange;
        private Path path;

        public override void OnStart()
        {
            visionRange = enemyScript.visionCollider;
        }

        public override TaskStatus OnUpdate()
        {
            //Debug.Log("Checking can see player condition");

            if (enemyScript.enemyStateList.IsSeeking)
                return TaskStatus.Success;

            if (enemyScript.IsOverlapping(visionRange))
            {
                GraphNode node1 = AstarPath.active.GetNearest(rb.position).node;
                GraphNode node2 = AstarPath.active.GetNearest(player.transform.position).node;

                if (PathUtilities.IsPathPossible(node1, node2))
                {
                    //Debug.Log("Path is possible, can see player");
                    enemyScript.CombatCoordinator.AddEnemyToCombatList(enemyScript);
                    return TaskStatus.Success;
                }
                else
                {
                    enemyScript.CombatCoordinator.RemoveEnemyFromCombatList(enemyScript);
                    return TaskStatus.Failure;
                }
                
            }
            else
            {
                enemyScript.CombatCoordinator.RemoveEnemyFromCombatList(enemyScript);
                return TaskStatus.Failure;                
            }
        }      
    }
}


