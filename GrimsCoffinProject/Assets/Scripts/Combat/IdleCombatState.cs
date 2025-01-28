using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleCombatState : CState
{
    //Variables
    [SerializeField] protected float duration;
    protected Animator animator;

    // Player Animation Stuff
    protected Animator playerAnimator_T; // Top
    protected Animator playerAnimator_B; // Bottom

    protected PlayerCombat playerCombat;

    public override void OnEnter(CStateMachine _stateMachine)
    {
        //Debug.Log("Is entering idle");
        base.OnEnter(_stateMachine);

        playerCombat = _stateMachine.GetComponent<PlayerCombat>();
        animator = playerCombat.scytheAnimator;

        animator.SetTrigger("Idle");
    }
}
