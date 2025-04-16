using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayerCombat : MonoBehaviour
{
    //Direction of attack, input of the left analog stick
    public enum AttackDirection
    {
        Side,
        Up,
        Down,
        Dash,
        Throw,
        Empty
    }

    //Data & Variables ------------------------------------------------------------------
    #region Data & Variables
    //References to needed items
    private CStateMachine meleeStateMachine;
    private PlayerStateList playerState;
    private PlayerControllerForces playerController;

    //Scythe objects
    public Collider2D hitbox;
    public Animator scytheAnimator;
    public GameObject scytheSprite;

    // Player Animation Stuff
    public Animator animator;

    //Timers
    private float lastComboTime;
    private float attackDurationTime;
    private float queueTimer;
    private float upAttackComboTime;

    public float AttackDurationTime { get { return attackDurationTime; } set { attackDurationTime = value; } }
    public float QueueTimer { get { return queueTimer; } set { queueTimer = value; } }
    public float LastComboTime { get { return lastComboTime; } set { lastComboTime = value; } }
    public float UpAttackComboTime { get {return upAttackComboTime; } set { upAttackComboTime = value; } }
    //[SerializeField] public float HoldAttackTimer;
    [SerializeField] private float holdAttackTimer; //{ get { return lastComboTime; } set { lastComboTime = value; } }


    //Attack
    [SerializeField] private bool attackAnimTotal;
    [SerializeField] private bool canAerialCombo;
    [SerializeField] private bool isAerialCombo;
    [SerializeField] private bool isAerialAttacking;
    [SerializeField] private bool isComboing;
    [SerializeField] public bool isDownAttacking;
    [SerializeField] private int attackClickCounter;
    [SerializeField] private int comboQueueLeft;
    [SerializeField] private int currentAttackAmount;
    [SerializeField] private List<AttackDirection> attackQueue;


    private AttackDirection attackDirection;

    [SerializeField] private bool isHoldingAttacking;
    public bool IsHoldingAttacking { get { return isHoldingAttacking; } }

    [SerializeField] private bool isInterruptingCombo;


    //Get Setters
    public bool CanAerialCombo
    {
        get { return canAerialCombo; }
        set { canAerialCombo = value; }
    }

    public bool IsAerialCombo
    {
        get { return isAerialCombo; }
        set { isAerialCombo = value; }
    }

    public bool IsAerialAttacking
    {
        get { return isAerialAttacking; }
        set { isAerialAttacking = value; }
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

    public AttackDirection CurrentAttackDirection { get { return attackDirection; } set { attackDirection = value; } }
    #endregion

    //Runtime Methods -------------------------------------------------------------------
    #region Runtime
    void Start()
    {
        //Get all required components
        meleeStateMachine = GetComponent<CStateMachine>();
        playerState = GetComponent<PlayerStateList>();
        playerController = GetComponent<PlayerControllerForces>();
        attackQueue = new List<AttackDirection>();

        if (scytheAnimator == null)
        {
            scytheAnimator = GameObject.Find("Scythe").GetComponent<Animator>();
        }

        //Make sure states are starting correctly
        canAerialCombo = true;
        isAerialCombo = false;
        isInterruptingCombo = false;
        
        meleeStateMachine.SetNextStateToMain();
    }

    private void FixedUpdate()
    {
        //Update proper values
        UpdateTimers();
        UpdateAttackVariables();

        // Refactored ground down attack logic
        if(CheckAttackDirection() == AttackDirection.Down && playerController.Grounded() && !isDownAttacking)
        {
            isDownAttacking = true;
            meleeStateMachine.SetNextState(new GroundDownCharge());
            AttackDurationTime = playerController.Data.gdHoldDuration;
            PlayerControllerForces.Instance.ExecuteDownAttack(true);
        }
        else if(isDownAttacking && CheckAttackDirection() != AttackDirection.Down)
        {
            isDownAttacking = false;
        }

        //Check to see if it should move on to the next combo
        if ((currentAttackAmount < playerController.Data.comboTotal && comboQueueLeft > 0 && AttackDurationTime < 0) || isInterruptingCombo)
        {
            //If the next attack que is a side attack, progress with the combo
            if (attackQueue[0] == AttackDirection.Side)
            {
                ComboAttack();

                //Remove from queue
                comboQueueLeft--;
                attackQueue.RemoveAt(0);

                //If there is more left in the combo, reset the queue timer to continue combo/queue
                if (comboQueueLeft > 0)
                    QueueTimer = playerController.Data.attackBufferTime;
            }
            //If the next attack is a directional attack, break out of the combo
            else if(isInterruptingCombo)
            {
                //Debug.Log("Is interrupting");
                playerController.EndSleep();

                //Switch for up or down attack
                switch (attackQueue[0])
                {
                    case AttackDirection.Up:
                        UpAttack();
                        break;
                    case AttackDirection.Down:
                        DownAttack();
                        break;
                    case AttackDirection.Dash:
                        Dash();
                        break;
                    case AttackDirection.Throw:
                        Throw();
                        break;
                }

                //Remove from queue
                attackQueue.Clear();

                //Fully reset combo because it was broken out of
                ResetCombo();
            }
        }
    }
    #endregion

    //Input Methods ---------------------------------------------------------------------
    #region Input Events
    //Attack Input
    private void OnAttack(InputValue value)
    {
        if (PlayerControllerForces.Instance.scytheThrown)
            return;

        //Make sure the attack is not being held so that it can execute a new input 
/*        if (!isHoldingAttacking)
        {*/
            if (value.isPressed && LastComboTime < 0)
            {
                //Make sure player is not dashing or the time scale is not zero so that the player cannot attack
                if (playerState.IsDashing || Time.timeScale == 0 || playerState.IsSliding)
                    return;

                //Check attack direction
                attackDirection = CheckAttackDirection();

                //Execute attack based off the direction of the player input
                scytheAnimator.ResetTrigger("Idle");
                switch (attackDirection)
                {
                    case AttackDirection.Up:
                        UpAttackCheck();
                        break;
                    case AttackDirection.Down:
                        //Sets holding true since the state is currently pressed
                        if (isDownAttacking)
                        {
                            DownAttackCheck();
                        }
                        //isHoldingAttacking = true;
                      
                        break;
                    case AttackDirection.Side:
                        BaseAttackCheck();
                        break;
                }
            }
        //}
        //Release attack input and reset corresponding variables
/*        else if (!value.isPressed)
        {
            isHoldingAttacking = false;
            holdAttackTimer = 0;
        }*/
    }

    //Dash Input
    private void OnDash()
    {
        //If the player is currently comboing, interrupt it //isComboing && 
        if (playerState.IsAttacking && playerController.Data.canDash)
        {
            InterruptCombo(AttackDirection.Dash, true);
        }  
    }

    private void OnAbility()
    {
        //Debug.Log("Trying to throw");
        if (isComboing && playerController.Data.canScytheThrow)
        {
            if (playerController.currentSP <= 0 || playerController.scytheThrown || !playerController.Data.canScytheThrow)
                return;

            InterruptCombo(AttackDirection.Throw,true);
        }
    }
    #endregion

    //Attack Checks to see if/when an attack should execute -----------------------------
    #region Attack Checks
    private void BaseAttackCheck()
    {
/*        if (meleeStateMachine.CurrentState.GetType() == null)
            return;*/

        //Debug.Log(meleeStateMachine.CurrentState.GetType());

        //Check for combo timer, if the click amount is less then combo total 
        if (LastComboTime < 0 && attackClickCounter < playerController.Data.comboTotal &&
            //Check if the attack counter is above, make sure the queue timer still allows for adding an attack
            ((attackClickCounter > 0 && QueueTimer > 0) || (attackClickCounter == 0 && attackDurationTime < 0)))
        {
            //Continue the combo queue
            if (attackClickCounter >= 1)
            {
                //Debug.Log("Should be adding to the combo queue timer");
                comboQueueLeft++;
                attackQueue.Add(AttackDirection.Side);

                //Add to the click counter
                attackClickCounter++;
                QueueTimer = playerController.Data.attackBufferTime;
            }
            //Run the first attack and add to the combo queue
            else if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
            {
                //Run the base attack
                BaseAttack();

                //Make sure the queue is cleared
                attackQueue.Clear();

                //Add to the click counter
                attackClickCounter++;
                QueueTimer = playerController.Data.attackBufferTime;
            }
        }
    }

    private void UpAttackCheck()
    {
        //If the player is currently comboing, interrupt it
        if (isComboing)
        {
            InterruptCombo(AttackDirection.Up, true);
        }
        else if (attackDurationTime < 0 && upAttackComboTime < 0)
        {
            UpAttack();
        }
    }

    private void DownAttackCheck()
    {
        //If the player is currently comboing, interrupt it
        if (isComboing)
        {
            InterruptCombo(AttackDirection.Down, true);
        }
        else if (attackDurationTime < 0)
        {
            meleeStateMachine.RegisteredAttack = true;
            //DownAttack();
        }
    }
    #endregion

    //Execute Attacks -------------------------------------------------------------------
    #region Attack Executions
    //Base single attack
    private void BaseAttack()
    {
        //Debug.Log("Base Attack");
        //If idle, enter the entry state
        meleeStateMachine.SetNextState(new MeleeEntryState());
        AttackDurationTime = playerController.Data.attackBufferTime;

        playerState.IsAttacking = true;
        isComboing = true;

        PlayerControllerForces.Instance.ExecuteBasicAttack();
        currentAttackAmount++;
    }

    //Combo attack, handles everything after the first attack
    private void ComboAttack()
    {
        //Debug.Log("Combo Attack");

        playerState.IsAttacking = true;
        meleeStateMachine.RegisteredAttack = true;
        isComboing = true;

        AttackDurationTime = playerController.Data.attackBufferTime;

        PlayerControllerForces.Instance.ExecuteBasicAttack();
        currentAttackAmount++;
    }

    private void UpAttack()
    {
        playerState.IsAttacking = true;
        //Up air attack
        /*if (!playerController.Grounded() && !playerController.Data.canAUpAttack)
        {
            //Debug.Log("Up Aerial Attack");
            meleeStateMachine.SetNextState(new AirUpState());
            AttackDurationTime = playerController.Data.aUpAttackDuration;
            isAerialAttacking = true;
            //PlayerControllerForces.Instance.StartAttack();

            PlayerControllerForces.Instance.ExecuteUpAttack(true);
        }
        //Up ground attack
        else */
        if (playerController.Data.canGUpAttack)
        {
            //Debug.Log("Up Ground Attack");
            meleeStateMachine.SetNextState(new GroundUpState());
            AttackDurationTime = playerController.Data.gUpAttackDuration;
            upAttackComboTime = playerController.Data.upAttackDelay;

            PlayerControllerForces.Instance.ExecuteUpAttack(true);
        }
    }

    private void DownAttack()
    {
        playerState.IsAttacking = true;
        if (!playerController.Grounded() && playerController.Data.canADownAttack)
        {
            //Debug.Log("Down Aerial Attack");
            isAerialAttacking = true;
            meleeStateMachine.SetNextState(new AirDownState());
            AttackDurationTime = playerController.Data.aDownAttackDuration;

            PlayerControllerForces.Instance.ExecuteDownAttack(false);
        }
        else if (playerController.Data.canGDownAttack)
        {
            //Debug.Log("Down Ground Attack");
            meleeStateMachine.SetNextState(new GroundDownCharge());
            AttackDurationTime = playerController.Data.gdHoldDuration;

            PlayerControllerForces.Instance.ExecuteDownAttack(true);
        }
    }

    private void Dash()
    {
        //Debug.Log("Executing dash: " + playerController.Data.dashInputBufferTime);
        playerController.LastPressedDashTime = playerController.Data.dashInputBufferTime;
    }

    private void Throw()
    {
        playerController.ExecuteScytheThrow();
    }
    #endregion

    //Helper Methods --------------------------------------------------------------------
    #region Helper Methods
    //Interrupt combo with another attack
    protected void InterruptCombo(AttackDirection nextAttackDir, bool canResetTimer = false)
    {
        if (!isInterruptingCombo)
        {
            attackQueue.Clear();
            attackQueue.Add(nextAttackDir);
            comboQueueLeft = 1;
            isInterruptingCombo = true;
        }
    }
    
    //Reset combo stats
    public void ResetCombo()
    {
        LastComboTime = playerController.Data.comboCooldownTime;
        attackClickCounter = 0;
        currentAttackAmount = 0;
        comboQueueLeft = 0;
        isComboing = false;

        QueueTimer = 0;
        attackDurationTime = 0;
        isInterruptingCombo = false;
        //meleeStateMachine.SetNextStateToMain();
    }

    //Check to see if the combo should be reset
    public bool ShouldResetCombo()
    {
        return AttackDurationTime < 0 && QueueTimer < 0 && comboQueueLeft == 0 && !isDownAttacking;
    }

    //Set attack direction based off the y direction of the left analog stick
    protected AttackDirection CheckAttackDirection()
    {
        //Debug.Log("Checking attack direction: " + playerController.MoveInput.y);
        if (playerController.MoveInput.y > playerController.Data.attackDirectionDeadzone)// && BelowXDeadzone())
            return AttackDirection.Up;
        else if (playerController.MoveInput.y < -playerController.Data.attackDirectionDeadzone)// && BelowXDeadzone())
            return AttackDirection.Down;
        else
            return AttackDirection.Side;
    }

    private bool BelowXDeadzone()
    {
        return playerController.MoveInput.x < playerController.Data.deadzone && playerController.MoveInput.x > -playerController.Data.deadzone;
    }
    #endregion

    //Timer and Variable Updates --------------------------------------------------------
    #region Timer and Variable Updates
    //Update timers in FixedUpdate
    private void UpdateTimers()
    {
        LastComboTime -= Time.deltaTime;
        AttackDurationTime -= Time.deltaTime;
        QueueTimer -= Time.deltaTime;
        upAttackComboTime -= Time.deltaTime;

        if(isHoldingAttacking)
        {
            holdAttackTimer += Time.deltaTime;
        }
    }

    //Updates all stats in FixedUpdate
    private void UpdateAttackVariables()
    {
        if (AttackDurationTime > 0)
            playerState.IsAttacking = true;  
        else if (!isDownAttacking) // isDownAttacking only for ground
        {
            playerState.IsAttacking = false;
        }

        //Reset combo if they have not attacked in a specific amount of time
        if (ShouldResetCombo() && AttackClickCounter > 0)
        {
            AttackClickCounter = 0;
            ResetCombo();

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
    #endregion
}
