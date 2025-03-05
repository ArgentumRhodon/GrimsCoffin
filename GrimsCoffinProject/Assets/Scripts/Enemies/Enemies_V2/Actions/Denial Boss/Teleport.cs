using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Animations;

namespace Core.AI
{
    public class Teleport : EnemyAction
    {
        public string animationTeleportName;
        public string animationAppearName;

        public float waitTimer; //Time before starting teleport animation
        public float awayDuration; //Time while away

        public float teleportOffset; //Offset used for player teleport

        public bool teleportToPlayer; //Either teleports to the player or to the side of the map

        //Private helper variables
        private bool teleportCompleted;
        private float targetLoc;

        //Cast to denial boss
        private DenialBoss bossScript;

        public override void OnStart()
        {
            teleportCompleted = false;
            bossScript = (DenialBoss)enemyScript;

            DOVirtual.DelayedCall(waitTimer, ExecuteTeleport);
        }

        public override TaskStatus OnUpdate()
        {
            if(teleportCompleted)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }

        //Teleport Execution
        //Start teleport
        private void ExecuteTeleport()
        {
            animator.Play(animationTeleportName);
            DOVirtual.DelayedCall(.3f, DisableHitbox);
            DOVirtual.DelayedCall(1 + awayDuration, TeleportLocation);

        }

        //Move to new location
        private void TeleportLocation()
        {
            if (teleportToPlayer)
            {
                transform.position = CalculatePlayerTele();
            }
            else
            {
                transform.position = CalculateEdgeTele();
            }
            enemyScript.TurnToPlayer();

            DOVirtual.DelayedCall(.1f, Appear);
        }

        //Reappear
        private void Appear()
        {
            animator.Play(animationAppearName);
            bossScript.GetComponent<TeamComponent>().teamIndex = TeamIndex.Enemy;
            DOVirtual.DelayedCall(.3f, FinishTeleport);
        }

        //Finish teleport sequence
        private void FinishTeleport()
        {
            teleportCompleted = true;
            bossScript.kinematicCollider.enabled = true;
        }

        //Helper Methods --------------------------------------------------------------
        //Return location to teleport next to player
        private Vector2 CalculatePlayerTele()
        {
            int direction;
            if (IsLeftBoundFurthest())
                direction = -1;
            else
                direction = 1;

            return new Vector2(player.transform.position.x + (teleportOffset * direction), transform.position.y);
        }

        //Return location to teleport next to edge
        private Vector2 CalculateEdgeTele()
        {
            //Teleport to edge that is further from the player
            bool teleLeft = IsLeftBoundFurthest();

            //Teleport Left
            if (teleLeft)
                return new Vector2(bossScript.roomBounds.bounds.min.x, transform.position.y);
            //Teleport Right
            else
                return new Vector2(bossScript.roomBounds.bounds.max.x, transform.position.y);
        }

        //Returns true if the left bound is furthest from the player
        private bool IsLeftBoundFurthest()
        {
            //Check to see which edge is further away from the player and 
            float playerDistanceMin = Mathf.Abs(player.transform.position.x - bossScript.roomBounds.bounds.min.x);
            float playerDistanceMax = Mathf.Abs(player.transform.position.x - bossScript.roomBounds.bounds.max.x);

            //Teleport to edge that is further from the player
            return playerDistanceMin > playerDistanceMax ? true : false;
        }

        private void DisableHitbox()
        {
            bossScript.kinematicCollider.enabled = false;
            bossScript.GetComponent<TeamComponent>().teamIndex = TeamIndex.Neutral;
        }
    }
}
