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
        public float horizontalForce = 5f;
        public float jumpForce = 10f;

        public float buildupTime;
        public float jumpTime;

        public string animationTriggerName;
        public string animationEndTriggerName; 

        private bool isGrounded;
        private bool hasJumped;

        private SpriteRenderer spriteRenderer;
        private float enemyHalfHeight;

        public override void OnStart()
        {
            DOVirtual.DelayedCall(buildupTime, StartJump, false);           
            animator.Play(animationTriggerName);


            spriteRenderer = animator.gameObject.GetComponent<SpriteRenderer>();
            enemyHalfHeight = spriteRenderer.bounds.extents.y / 2;
        }

        private void StartJump()
        {
            int direction = enemyScript.GetPlayerXDirection();
            rb.AddForce(new Vector2(horizontalForce * direction, jumpForce), ForceMode2D.Impulse);
            hasJumped = true;
        }

        public override TaskStatus OnUpdate()
        {
            if(!hasJumped)
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

        private bool IsGrounded()
        {
            Debug.DrawRay(transform.position, Vector2.down * (enemyHalfHeight + 0.1f), Color.red);
            return Physics2D.Raycast(transform.position, Vector2.down, (enemyHalfHeight + 0.1f), LayerMask.GetMask("Ground"));            
        }

        public override void OnEnd()
        {
            animator.SetTrigger(animationEndTriggerName);
            base.OnEnd();
        }
    }
}
