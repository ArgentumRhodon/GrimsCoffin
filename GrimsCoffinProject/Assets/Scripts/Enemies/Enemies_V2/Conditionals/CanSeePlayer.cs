using Core.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI 
{
    public class CanSeePlayer : EnemyConditional
    {
        //Collider for the player's vision
        private Collider2D visionRange;

        public override void OnStart()
        {
            visionRange = enemyScript.visionCollider;
        }

        public override TaskStatus OnUpdate()
        {
            return IsOverlapping() ? TaskStatus.Success : TaskStatus.Failure;
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


