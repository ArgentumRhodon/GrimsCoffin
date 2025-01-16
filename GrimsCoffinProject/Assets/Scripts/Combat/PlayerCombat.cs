using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCombat : MonoBehaviour
{
    protected enum AttackDirection 
    { 
        None, 
        Up, 
        Down
    }


    //References to needed items
    private CStateMachine meleeStateMachine;
    private PlayerStateList playerState;
    private PlayerControllerForces playerController;
    public PlayerData Data;

    //Scythe objects
    public Collider2D hitbox;
    public Animator scytheAnimator;

    // Player Animation Stuff
    public Animator animator_T; // Top
    public Animator animator_B; // Bottom

    //Timers
    private float lastComboTime;
    [SerializeField] private float attackDurationTime;
    [SerializeField] private float queueTimer;

    public float AttackDurationTime { get { return attackDurationTime; } set { attackDurationTime = value; } }
    public float QueueTimer { get { return queueTimer; } set { queueTimer = value; } }
    public float LastComboTime { get { return lastComboTime; } set { lastComboTime = value; } }

    //Attack
    [SerializeField] private bool canAerialCombo;
    [SerializeField] private bool isAerialCombo;
    [SerializeField] private bool isComboing;
    [SerializeField] private int attackClickCounter;
    [SerializeField] private int attackQueueLeft;
    [SerializeField] private int currentAttackAmount;

    [SerializeField] private List<AttackDirection> attackQueue;


    private AttackDirection attackDirection;

    //Get Setters
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

    public bool IsComboing
    {
        get { return isComboing; }
        set { isComboing = value; }
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
        playerController = GetComponent<PlayerControllerForces>();
        Data = playerController.Data;

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
        if (currentAttackAmount < Data.comboTotal && attackQueueLeft > 0 && AttackDurationTime < 0)
        {
            ComboAttack();
            attackQueueLeft--;

            if (attackQueueLeft > 0)
                QueueTimer = Data.attackBufferTime;
        }
        
    }

    private void OnAttack()
    {
        //Debug.Log("Attack is running");
        if (playerState.IsDashing || Time.timeScale == 0)
            return;

        switch (attackDirection)
        {
            case AttackDirection.Up:
                UpAttackCheck();
                break;
            case AttackDirection.Down:
                DownAttackCheck();
                break;
            case AttackDirection.None:
                BaseAttackCheck();
                break;
        }




        /*//Check for combo timer, if the click amount is less then combo total 
        if (LastComboTime < 0 && attackClickCounter < Data.comboTotal &&
            //Check if the attack counter is above, make sure the queue timer still allows for adding an attack
            ((attackClickCounter > 0 && QueueTimer > 0) || attackClickCounter == 0))
        {
            attackDirection = CheckAttackDirection();

            if (attackDirection == AttackDirection.None)
            {
                //Add to the click counter
                attackClickCounter++;
                QueueTimer = Data.attackBufferTime;

                //Continue the combo queue
                if (attackClickCounter > 1)
                {
                    //Debug.Log("Should be adding to the combo queue timer");
                    attackQueueLeft++;
                }
                //Run the first attack and add to the combo queue
                else
                {
                    BaseAttack();
                }
            }
            else if (!isComboing)
            {
                switch (attackDirection)
                {
                    case AttackDirection.Up:
                        UpAttack();
                        break;
                    case AttackDirection.Down:
                        DownAttack();
                        break;
                }
            }
        }*/


    }


    //Attack Checks to see if/when an attack should execute
    private void BaseAttackCheck()
    {
        //Check for combo timer, if the click amount is less then combo total 
        if (LastComboTime < 0 && attackClickCounter < Data.comboTotal &&
            //Check if the attack counter is above, make sure the queue timer still allows for adding an attack
            ((attackClickCounter > 0 && QueueTimer > 0) || attackClickCounter == 0))
        {
            attackDirection = CheckAttackDirection();

            if (attackDirection == AttackDirection.None)
            {
                //Add to the click counter
                attackClickCounter++;
                QueueTimer = Data.attackBufferTime;

                //Continue the combo queue
                if (attackClickCounter > 1)
                {
                    //Debug.Log("Should be adding to the combo queue timer");
                    attackQueueLeft++;
                    attackQueue.Add(AttackDirection.None);
                }
                //Run the first attack and add to the combo queue
                else if(attackDurationTime < 0)
                {
                    BaseAttack();
                }
            }
        }
    }

    private void UpAttackCheck()
    {

    }

    private void DownAttackCheck()
    {

    }


    //Execute Attacks ------------------------------------------------------------------------
    #region Attack Executions
    //Base single attack
    private void BaseAttack()
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
        isComboing = true;

        AttackDurationTime = Data.attackBufferTime;
             
        PlayerControllerForces.Instance.StartAttack();
        currentAttackAmount++;
    }

    private void UpAttack()
    {
        if (playerController.Grounded())
        {
            Debug.Log("Up Ground Attack");
            //meleeStateMachine.SetNextState(new GroundUpState());
            AttackDurationTime = Data.gUpAttackDuration;

            //PlayerControllerForces.Instance.StartAttack();
        }
        else
        {
            Debug.Log("Up Aerial Attack");
            //meleeStateMachine.SetNextState(new AirUpState());
            AttackDurationTime = Data.aUpAttackDuration;

            //PlayerControllerForces.Instance.StartAttack();
        }
    }

    private void DownAttack()
    {
        if (playerController.Grounded())
        {
            Debug.Log("Down Ground Attack");
            //meleeStateMachine.SetNextState(new GroundDownState());
            AttackDurationTime = Data.gDownAttackDuration;

            //PlayerControllerForces.Instance.StartAttack();
        }
        else
        {
            Debug.Log("Down Aerial Attack");
            //meleeStateMachine.SetNextState(new AirDownState());
            AttackDurationTime = Data.aDownAttackDuration;

            //PlayerControllerForces.Instance.StartAttack();
        }
    }
    #endregion


    //Interrupt combo with another attack
    protected void InterruptCombo(AttackDirection nextAttackDir)
    {

    }
    
    //Reset combo stats
    public void ResetCombo()
    {
        LastComboTime = Data.comboSleepTime;
        attackClickCounter = 0;
        currentAttackAmount = 0;
        attackQueueLeft = 0;
        isComboing = false;

        QueueTimer = 0;
        attackDurationTime = 0;
    }

    public bool ShouldResetCombo()
    {
        //return attackDurationTime < 0 && queueTimer < 0;// && attackQueueLeft == 0;
        return AttackDurationTime < 0 && QueueTimer < 0 && attackQueueLeft == 0;
    }

    protected AttackDirection CheckAttackDirection()
    {
        //Debug.Log("Checking attack direction: " + playerController.MoveInput.y);
        if (playerController.MoveInput.y > Data.attackDirectionDeadzone)// && BelowXDeadzone())
            return AttackDirection.Up;
        else if (playerController.MoveInput.y < -Data.attackDirectionDeadzone)// && BelowXDeadzone())
            return AttackDirection.Down;
        else
            return AttackDirection.None;
    }

    private bool BelowXDeadzone()
    {
        return playerController.MoveInput.x < Data.deadzone && playerController.MoveInput.x > -Data.deadzone;
    }

    //Update timers in FixedUpdate
    private void UpdateTimers()
    {
        LastComboTime -= Time.deltaTime;
        AttackDurationTime -= Time.deltaTime;
        QueueTimer -= Time.deltaTime;
    }

    //Updates all stats in FixedUpdate
    private void UpdateAttackVariables()
    {
        if (AttackDurationTime > 0)
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
