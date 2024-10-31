using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private CStateMachine meleeStateMachine;

    public Collider2D hitbox;
    public Animator scytheAnimator;

    // Input buffer Timer
    public float attackPressedTimer = 0;

    void Start()
    {
        meleeStateMachine = GetComponent<CStateMachine>();
    }

    private void FixedUpdate()
    {
        if(attackPressedTimer > 0) 
            attackPressedTimer -= Time.deltaTime;
    }

    private void OnAttack()
    {
        //Debug.Log("Attack Registered");
        if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new MeleeEntryState());
            attackPressedTimer = 2f;
        } 
        else if (attackPressedTimer > 0)
        {
            meleeStateMachine.RegisteredAttack = true;
            attackPressedTimer = 2f;
        }

    }
}
