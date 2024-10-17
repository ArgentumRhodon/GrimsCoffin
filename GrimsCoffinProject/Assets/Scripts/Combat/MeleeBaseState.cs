using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : CState
{
    //Variables
    [SerializeField] protected float duration;
    protected Animator animator;

    //Bool to check if it should continue the combo or not
    protected bool shouldCombo;
    //Index of sequence in attack
    protected int attackIndex;



    //Cached hit collider component of this attack
    //protected Collider2D hitCollider;
    //Cached already struck objects of said attack to avoid overlapping attacks on same target
    //private List<Collider2D> collidersDamaged;


    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = _stateMachine.GetComponent<PlayerCombat>().scytheAnimator;
        //collidersDamaged = new List<Collider2D>();
        //hitCollider = _stateMachine.GetComponent<PlayerCombat>().hitbox;
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);
        _stateMachine.GetComponent<PlayerCombat>().attackPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }

        if (_stateMachine.GetComponent<PlayerCombat>().attackPressedTimer > 0) //animator.GetFloat("AttackWindow.Open") > 0f &&
        {
            shouldCombo = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    protected void Attack()
    {
        //Attack the enemy, check for collisions
    }
}
