using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;

namespace Core.AI
{
    public class EnemyConditional : Conditional
    {
        [Header("GameObjects")]
        protected Rigidbody2D rb;
        protected Animator animator;
        protected TeamComponent team;
        protected PlayerControllerForces player;
        protected Enemy enemyScript;
        protected Seeker seeker;

        public override void OnAwake()
        {
            rb = GetComponent<Rigidbody2D>();
            player = PlayerControllerForces.Instance;
            team = GetComponent<TeamComponent>();
            animator = gameObject.GetComponentInChildren<Animator>();
            enemyScript = GetComponent<Enemy>();
            seeker = GetComponent<Seeker>();
        }
    }
}
