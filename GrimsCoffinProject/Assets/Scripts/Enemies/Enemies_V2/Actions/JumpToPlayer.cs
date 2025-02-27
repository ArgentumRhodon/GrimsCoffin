using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Core.AI
{
    public class JumpToPlayer : EnemyAction
    {
        public float offset = 0;

        public float height = 2;
        private float gravity;

        public float buildupTime;

        public string animationStartingName;
        public string animationRunningTriggerName;
        public string animationEndTriggerName;

        private bool isGrounded;
        private bool hasJumped;

        private SpriteRenderer spriteRenderer;
        private float enemyHalfHeight;

        public override void OnStart()
        {
            //Set gravity scale based off of the rigid body
            gravity = -rb.gravityScale * 9.8f;

            //Face player
            enemyScript.TurnToPlayer();

            //Start jump
            animator.Play(animationStartingName);
            DOVirtual.DelayedCall(buildupTime, Jump, false);

            //Ground check for animation
            spriteRenderer = animator.gameObject.GetComponent<SpriteRenderer>();
            enemyHalfHeight = spriteRenderer.bounds.extents.y / 2;          
        }

        public override TaskStatus OnUpdate()
        {
            if(IsCloseToGround() && hasJumped)
                animator.SetTrigger(animationEndTriggerName);

            if (!hasJumped)
            {
                return TaskStatus.Running;
            }
            else
            {
                if (IsGrounded())
                {
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Running;
                }
            }
        }

        private void Jump()
        {
            //Face player
            enemyScript.TurnToPlayer();

            rb.AddForce(CalculateLaunchVelocity(), ForceMode2D.Impulse);
            animator.SetTrigger(animationRunningTriggerName);
            DOVirtual.DelayedCall(.2f, EnabledJump, false);
        }

        private void EnabledJump()
        {
            hasJumped = true;
        }

        private Vector2 CalculateLaunchVelocity()
        {
            float xOffset = enemyScript.FindPlayerDirection().x > 0 ? offset * -1 : offset;

            float displacementY = player.transform.position.y - transform.position.y;
            float displacementX = player.transform.position.x - transform.position.x + xOffset;

            Vector2 velocityY = Vector2.up * Mathf.Sqrt(-2 * gravity * height);
            Vector2 velocityX = Vector2.right * displacementX / (Mathf.Sqrt(-2 * height / gravity));

            //Debug.Log(velocityY + velocityX);

            return velocityY + velocityX;
        }

        private bool IsGrounded()
        {
            //Debug.DrawRay(transform.position, Vector2.down * (enemyHalfHeight + 0.1f), Color.red);
            return Physics2D.Raycast(transform.position, Vector2.down, (enemyHalfHeight + 0.1f), LayerMask.GetMask("Ground"));
        }

        private bool IsCloseToGround()
        {
            Debug.DrawRay(transform.position, Vector2.down * (enemyHalfHeight * 1.1f), Color.red);
            return Physics2D.Raycast(transform.position, Vector2.down, (enemyHalfHeight * 1.1f), LayerMask.GetMask("Ground"));
        }

        public override void OnEnd()
        {
            //animator.SetTrigger(animationEndTriggerName);
            isGrounded = true;
            hasJumped = false;
        }
    }
}
