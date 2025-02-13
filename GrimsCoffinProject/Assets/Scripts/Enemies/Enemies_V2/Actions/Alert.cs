using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace Core.AI
{
    public class Alert : EnemyAction
    {
        public string animationTriggerName;
        public float alertTimer;

        private bool isReady;

        // Start is called before the first frame update
        public override void OnStart()
        {
            isReady = false;
            enemyScript.TurnToPlayer();

            animator.Play(animationTriggerName);

            StartCoroutine(nameof(AlertTimer),alertTimer);
        }

        public override TaskStatus OnUpdate()
        {
            if (isReady)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }

        private IEnumerator AlertTimer(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            isReady = true;
        }
    }
}
