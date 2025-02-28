using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Core.AI
{
    public class Jump : EnemyAction
    {
        public float horizontalForce = 4f;
        public float verticalForce = 2f;

        public float buildupTime;

        public string animationStartingName;
        public string animationRunningTriggerName;
        public string animationEndTriggerName;

        public bool shouldWait;
        public float waitTimerMax = 0;
        private float waitTimer;

        public bool shouldFacePlayer;

        private bool isGrounded;
        private bool hasJumped;

        private SpriteRenderer spriteRenderer;
        private float enemyHalfHeight;

        public override void OnStart()
        {
            //Face player
            if(shouldFacePlayer)
                enemyScript.TurnToPlayer();

            //Start Jump
            animator.Play(animationStartingName);
            DOVirtual.DelayedCall(buildupTime, StartJump, false);

            //Ground check for animation
            spriteRenderer = animator.gameObject.GetComponent<SpriteRenderer>();
            enemyHalfHeight = spriteRenderer.bounds.extents.y / 2;
        }

        private void StartJump()
        {
            //Calculate jump
            int direction = enemyScript.GetPlayerXDirection();
            rb.AddForce(new Vector2(horizontalForce * direction, verticalForce), ForceMode2D.Impulse);
            animator.SetTrigger(animationRunningTriggerName);

            //animation delay check
            DOVirtual.DelayedCall(.15f, EnabledJump, false);

            if (shouldWait)
                waitTimer = waitTimerMax;
        }

        public override TaskStatus OnUpdate()
        {
            if (IsCloseToGround() && hasJumped)
                animator.SetTrigger(animationEndTriggerName);

            if (shouldWait && waitTimer > 0)
                waitTimer -= Time.deltaTime;

            if (!hasJumped)
            {
                return TaskStatus.Running;
            }
            else
            {
                if (IsGrounded())
                {
                    if (shouldWait)
                    {
                        if (waitTimer < 0)
                            return TaskStatus.Success;
                        else
                            return TaskStatus.Running;
                    }
                    else
                    {
                        return TaskStatus.Success;
                    }       
                }
                else
                {
                    return TaskStatus.Running;
                }
            }         
        }

        private void EnabledJump()
        {
            hasJumped = true;
        }

        private bool IsCloseToGround()
        {
            Debug.DrawRay(transform.position, Vector2.down * (enemyHalfHeight * 1.1f), Color.red);
            return Physics2D.Raycast(transform.position, Vector2.down, (enemyHalfHeight * 1.1f), LayerMask.GetMask("Ground"));
        }

        private bool IsGrounded()
        {
            //Debug.DrawRay(transform.position, Vector2.down * (enemyHalfHeight + 0.1f), Color.red);
            return Physics2D.Raycast(transform.position, Vector2.down, (enemyHalfHeight + 0.1f), LayerMask.GetMask("Ground"));            
        }

        public override void OnEnd()
        {
            isGrounded = true;
            hasJumped = false;
        }
    }
}
