using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class EnemyConditional : Conditional
    {
        protected Rigidbody2D body;
        protected Animator animator;
        protected TeamComponent team;
        protected PlayerControllerForces player;

        public override void OnAwake()
        {
            body = GetComponent<Rigidbody2D>();
            player = PlayerControllerForces.Instance;
            team = GetComponent<TeamComponent>();
            animator = gameObject.GetComponentInChildren<Animator>();
        }
    }
}
