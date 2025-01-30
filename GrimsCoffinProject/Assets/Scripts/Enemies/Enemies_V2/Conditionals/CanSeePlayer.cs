using Core.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI 
{
    public class CanSeePlayer : EnemyConditional
    {
        private Collider2D visionRange;
        private bool isColliding;

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

            for(int i = 0; i < colliderCount; i++)
            {
                if (collidersToCheck[i].gameObject.tag == "Player")
                    return true;
            }
            return false;
        }

/*        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
                isColliding = true;
        }*/
    }
}


