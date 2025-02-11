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
            if (IsOverlapping())
            {
                GraphNode node1 = AstarPath.active.GetNearest(rb.position).node;
                GraphNode node2 = AstarPath.active.GetNearest(player.transform.position).node;

                if (PathUtilities.IsPathPossible(node1, node2))
                {
                    return TaskStatus.Success;
                }                   
                else
                    return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Failure;                
            }
            //return IsOverlapping() ? TaskStatus.Success : TaskStatus.Failure;
        }

        public bool IsOverlapping()
        {            
            //Check for colliders overlapping
            Collider2D[] collidersToCheck = new Collider2D[10];
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            int colliderCount = Physics2D.OverlapCollider(visionRange, filter, collidersToCheck);

            //Go through all colliders and check to see if it is the player
            for(int i = 0; i < colliderCount; i++)
            {
                if (collidersToCheck[i].gameObject.tag == "Player")
                    return true;
            }
            return false;
        }
           
    }
}


