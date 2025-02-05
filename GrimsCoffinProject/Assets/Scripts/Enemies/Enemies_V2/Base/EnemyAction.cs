using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI 
{
    public class EnemyAction : Action
    {
        [Header("GameObjects")]
        protected Rigidbody2D rb;
        protected Animator animator;
        protected TeamComponent team;
        protected PlayerControllerForces player;
        protected BTEnemy enemyScript;

        public override void OnAwake()
        {
            rb = GetComponent<Rigidbody2D>();
            player = PlayerControllerForces.Instance;
            team = GetComponent<TeamComponent>();
            animator = gameObject.GetComponentInChildren<Animator>();
            enemyScript = GetComponent<BTEnemy>();
        }
    }
}


