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
    protected Collider2D hitCollider;
    //Cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider2D> collidersDamaged;


    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = _stateMachine.GetComponent<PlayerCombat>().scytheAnimator;
        collidersDamaged = new List<Collider2D>();
        hitCollider = _stateMachine.GetComponent<PlayerCombat>().hitbox;
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        base.OnUpdate(_stateMachine);
        _stateMachine.GetComponent<PlayerCombat>().attackPressedTimer -= Time.deltaTime;

/*        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }*/

/*        if (animator.GetFloat("AttackWindow.Open") > 0f)
        {
            shouldCombo = true;
        }*/

        if (_stateMachine.GetComponent<PlayerCombat>().attackPressedTimer > 0f)
        {
            shouldCombo = true;
            Attack();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    protected void Attack()
    {
        //Debug.Log("Attack Swing");
        //Attack the enemy, check for collisions
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    Debug.Log("Enemy Has Taken: " + attackIndex + " Damage");
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }

    }
}