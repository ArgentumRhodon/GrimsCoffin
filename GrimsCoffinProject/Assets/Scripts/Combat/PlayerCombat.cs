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

    [SerializeField] private float attackDurationTime;
    [SerializeField] private float queueTimer;

    public float AttackDurationTime { get { return attackDurationTime; } set { attackDurationTime = value; } }
    public float QueueTimer { get { return queueTimer; } set { queueTimer = value; } }

    //Attack
    [SerializeField] private bool canAerialCombo;
    [SerializeField] private bool isAerialCombo;
    [SerializeField] private int attackClickCounter;
    [SerializeField] private int attackQueueLeft;
    [SerializeField] private int currentAttackAmount;

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

    public int AttackClickCounter
    {
        get { return attackClickCounter; }
        set { attackClickCounter = value; }
    }

    void Start()
    {
        meleeStateMachine = GetComponent<CStateMachine>();
        playerState = GetComponent<PlayerStateList>();
        Data = GetComponent<PlayerControllerForces>().Data;

        if (scytheAnimator == null)
        {
            scytheAnimator = GameObject.Find("Scythe").GetComponent<Animator>();
        }

        canAerialCombo = true;
        isAerialCombo = false;
    }

    private void FixedUpdate()
    {
        //Update proper values
        UpdateTimers();
        UpdateAttackVariables();

        //Check to see if it should move on to the next combo
        if (currentAttackAmount < Data.comboTotal && attackQueueLeft > 0 && attackDurationTime < 0)
        {
            ComboAttack();
            attackQueueLeft--;

            if(attackQueueLeft > 0)
                queueTimer = Data.attackBufferTime;
        }
    }

    private void OnAttack()
    {
        //Debug.Log("Attack is running");
        if (playerState.IsDashing || Time.timeScale == 0)
            return;
      
        //Check for combo timer, if the click amount is less then combo total 
        if (LastComboTime < 0 && attackClickCounter < Data.comboTotal &&
            //Check if the attack counter is above, make sure the queue timer still allows for adding an attack
            ((attackClickCounter > 0 && queueTimer > 0) || attackClickCounter == 0))
        {
            //Add to the click counter
            attackClickCounter++;
            queueTimer = Data.attackBufferTime;

            //Continue the combo queue
            if (attackClickCounter > 1)
            {
                //Debug.Log("Should be adding to the combo queue timer");
                attackQueueLeft++;
            }
            //Run the first attack and add to the combo queue
            else
            {
                Attack();
            }
        }
    }

    //Base single attack
    private void Attack()
    {
        //If idle, enter the entry state
        if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new MeleeEntryState());
            AttackDurationTime = Data.attackBufferTime;

            PlayerControllerForces.Instance.StartAttack();
            currentAttackAmount++;
        }
    }

    //Combo attack, handles everything after the first attack
    private void ComboAttack()
    {
        meleeStateMachine.RegisteredAttack = true;
        AttackDurationTime = Data.attackBufferTime;
             
        PlayerControllerForces.Instance.StartAttack();
        currentAttackAmount++;
    }

    //Reset combo stats
    public void ResetCombo()
    {
        LastComboTime = Data.comboSleepTime;
        attackClickCounter = 0;
        currentAttackAmount = 0;

        queueTimer = 0;
        attackDurationTime = 0;
    }

    public bool ShouldResetCombo()
    {
        //return attackDurationTime < 0 && queueTimer < 0;// && attackQueueLeft == 0;
        return attackDurationTime < 0 && queueTimer < 0 && attackQueueLeft == 0;

    }

    //Update timers in FixedUpdate
    private void UpdateTimers()
    {
        if (attackPressedTimer > 0)
            attackPressedTimer -= Time.deltaTime;

        //Combat Timers
        LastComboTime -= Time.deltaTime;
        AttackDurationTime -= Time.deltaTime;
        queueTimer -= Time.deltaTime;
    }

    //Updates all stats in FixedUpdate
    private void UpdateAttackVariables()
    {
        if (attackDurationTime > 0)
            playerState.IsAttacking = true;           
        else
            playerState.IsAttacking = false;    

        //Reset combo if they have not attacked in a specific amount of time
        if (ShouldResetCombo() && AttackClickCounter > 0)
        {
            AttackClickCounter = 0;
            ResetCombo();

            animator_T.SetFloat("comboRatio", 0);
            animator_B.SetFloat("comboRatio", 0);

            //Update aerial combo values
            if(isAerialCombo)
            {
                canAerialCombo = false;
                isAerialCombo = false;
            }
        }

        if (LastComboTime < 0)
        {
            canAerialCombo = true;
        }
    }
}
