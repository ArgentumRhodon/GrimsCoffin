using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCombat : MonoBehaviour
{
    private CStateMachine meleeStateMachine;
    private PlayerStateList playerState;
    public PlayerData Data;

    public Collider2D hitbox;
    public Animator scytheAnimator;

    // Player Animation Stuff
    public Animator animator_T; // Top
    public Animator animator_B; // Bottom

    //Timers
    public float attackPressedTimer = 0;
    public float LastComboTime { get; set; }
    [SerializeField] private float lastAttackTime; 
    
    public float LastAttackTime { get { return lastAttackTime; } set { lastAttackTime = value; } }

    //Attack
    private bool canAerialCombo;
    private bool isAerialCombo;
    private int attackCounter;

    public bool CanAerialCombo
    {
        get { return canAerialCombo;}
        set { canAerialCombo = value; }
    }

    public bool IsAerialCombo
    {
        get { return isAerialCombo; }
        set { isAerialCombo = value; }
    }

    public int AttackCounter
    {
        get { return attackCounter; }
        set { attackCounter = value; }
    }

    void Start()
    {
        meleeStateMachine = GetComponent<CStateMachine>();
        canAerialCombo = true;
        playerState = GetComponent<PlayerStateList>();
        Data = GetComponent<PlayerControllerForces>().Data;

        if (scytheAnimator == null)
        {
            scytheAnimator = GameObject.Find("Scythe").GetComponent<Animator>();
        }
    }

    private void FixedUpdate()
    {
        if(attackPressedTimer > 0) 
            attackPressedTimer -= Time.deltaTime;

        //Combat
        LastComboTime -= Time.deltaTime;
        LastAttackTime -= Time.deltaTime;
    }

    private void OnAttack()
    {
        if (playerState.IsDashing || Time.timeScale == 0)
            return;

        //Check for player cooldown
        if (LastComboTime < 0)
        {
            //If idle, enter the entry state
            if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                meleeStateMachine.SetNextState(new MeleeEntryState());
                attackPressedTimer = 1.4f;
                LastAttackTime = Data.attackBufferTime;
            }
            //If not idle, register attack to move to next state
            else if (attackPressedTimer > 0)
            {
                meleeStateMachine.RegisteredAttack = true;
                LastAttackTime = Data.attackBufferTime;
            }
        }
    }

    public void ResetCombo()
    {
        LastComboTime = Data.comboSleepTime;
        //LastAttackTime = 0;
        AttackCounter = 0;
    }
}
