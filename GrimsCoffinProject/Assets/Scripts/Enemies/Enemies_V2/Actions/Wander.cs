using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Wander : EnemyAction
    {
        public string animationTriggerName;
        public string nextAnimationTrigger;

        private bool waitingForGround;
        private bool hasGrounded;

        private bool isDone;

        public override void OnStart()
        {
            //animator.SetTrigger(animationTriggerName);
            animator.Play(animationTriggerName);
            Walk(1f);
            isDone = false;
        }

        public override void OnFixedUpdate()
        {
            if (CheckFlip())
            {
                //animator.SetTrigger(nextAnimationTrigger);
                animator.Play(nextAnimationTrigger);
                rb.velocity = Vector2.zero;
                isDone = true;
            }
            else
            {
                Walk(1);
            }

        }

        public override TaskStatus OnUpdate()
        {
            if (isDone)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }


        private bool CheckFlip()
        {
            //Check to see if it is not colliding with the ground or is colliding with a wall
            if (!enemyScript.airChecker.IsColliding || enemyScript.wallChecker.IsColliding)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Walk(float lerpAmount)
        {
            //Calculate the direction and our desired velocity
            float targetSpeed = enemyScript.Direction * enemyScript.movementSpeed;       

            //Smooth changes to direction and speed using a lerp function
            targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);
         
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? enemyScript.movementAccelAmount : enemyScript.movementDeccelAmount;

            //Calculate difference between current velocity and desired velocity
            float speedDif = targetSpeed - rb.velocity.x;
            //Calculate force along x-axis to apply to thr player
            float movement = speedDif * accelRate;

            //Convert movement to a vector and apply it
            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }
    }
}
