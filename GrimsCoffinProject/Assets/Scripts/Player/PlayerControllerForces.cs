using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.GoogleVr;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using FMODUnity;
using Unity.VisualScripting;
using System.Threading.Tasks;
using FMOD.Studio;
using System;

public class PlayerControllerForces : MonoBehaviour
{
    //Data & Variables --------------------------------------------------------------------------------------------
    #region Data & Variables
    //Scriptable object which holds all data on player movement parameters
    public PlayerData Data;

    //Rigidbody and player state
    public Rigidbody2D rb { get; private set; }
    public PlayerStateList playerState;

    //Timers
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastHeldWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastJumpTime { get; private set; }
    public float LastWallJumpTime { get; private set; }

    //Walk
    [SerializeField] private float walkModifier;
    public float WalkModifier { get { return walkModifier; } set { walkModifier = value; } }
    [SerializeField] private bool canSleepWalk;
    public bool CanSleepWalk { get { return canSleepWalk; } set { canSleepWalk = value; } }

    //Jump
    private bool isJumpCancel;
    private bool isJumpFalling;
    private int airJumpCounter = 0;
    private float jumpVelocity;

    //Wall Jump
    private float wallJumpStartTime;
    private int lastWallJumpDir;

    //Dash
    private int dashesLeft;
    private bool dashRefilling;
    private Vector2 lastDashDir;
    private bool isDashAttacking;

    //Abilities
    [SerializeField] private GameObject scytheProjectilePrefab;
    [SerializeField] public GameObject heldScytheSprite;

    //Input parameters
    private Vector2 moveInput;
    public Vector2 MoveInput { get {return moveInput; } }

    // Animation Stuff
    [SerializeField] private Animator animator;
    [SerializeField] private Animator scytheAnimator; // Top

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; set; }

    //Positions used for state checks
    [Header("Tile Checks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform frontWallCheckPoint;
    [SerializeField] private Transform backWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);

    [Header("Player Stats")]
    [SerializeField] public float currentHP;
    [SerializeField] public float currentSP;
    [SerializeField] public float invincibilityTimer;
    [SerializeField] public bool hasInvincibility;
    [SerializeField] public bool hasDashInvincibility;
    [SerializeField] public bool scytheThrown;

    [Header("Player UI")]
    [SerializeField] public InteractionPrompt interactionPrompt;

    //Map Zoom States
    private bool zoomingMapIn;
    private bool zoomingMapOut;
    private bool panningMap;
    private bool movingOnMapOpen;

    private float currentTime;

    [Header("FMOD Events and Controller")]
    [Range(1f, 10f)]
    [Tooltip("This slide controls how fast you want your footstep speed to be")]
    [SerializeField] public float footstepSpeed = 3f;
    [Tooltip("FMOD events for the walking mechanism")]
    [SerializeField] public EventReference walkSFX;
    protected EventInstance walkInstance;

    [Tooltip("FMOD events for the jumping mechanism")]
    [SerializeField] public EventReference jumpSFX;
    protected EventInstance jumpInstance;

    [Tooltip("FMOD events for the wallJumping mechanism")]
    [SerializeField] public EventReference wallLeapSFX;
    protected EventInstance wallLeapInstance;

    [Tooltip("FMOD events for the land-after-jump mechanism")]
    [SerializeField] public EventReference landSFX;
    protected EventInstance landInstance;
    private bool FMODJumpFinished = false;
    private bool FMODIsLandedPlayed = false;

    [Tooltip("FMOD events for the dashing mechanism")]
    [SerializeField] public EventReference dashSfx;

    [Tooltip("FMOD events for the sliding mechanism")]
    [SerializeField] public EventReference slidingSFX;
    protected EventInstance slideInstance;
    private bool isSlidingPlayed = false;

    [Tooltip("FMOD events for the taking damage")]
    [SerializeField] public EventReference damageSFX;
    protected EventInstance damageInstance;


    //Singleton so the controller can be referenced across scripts
    public static PlayerControllerForces Instance;

    //Reference to Player Input/Controls
    private PlayerControls playerControls;
    private PlayerCombat playerCombat;

    //Time Variables
    private float localDeltaTime;
    private bool isSleeping;
    public bool IsSleeping { get { return isSleeping; } }
    #endregion

    //Runtime Methods ---------------------------------------------------------------------------------------------
    #region Runtime
    private void Awake()
    {
        //Set rigidbody
        rb = GetComponent<Rigidbody2D>();
        playerCombat = GetComponent<PlayerCombat>();

        //Set Instance so this script is called as a singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        //Set player controls to new input
        playerControls = new PlayerControls();

        //Create sound instances for player controller
        walkInstance = RuntimeManager.CreateInstance(walkSFX);
        jumpInstance = RuntimeManager.CreateInstance(jumpSFX);
        landInstance = RuntimeManager.CreateInstance(landSFX);
        slideInstance = RuntimeManager.CreateInstance(slidingSFX);
        wallLeapInstance = RuntimeManager.CreateInstance(wallLeapSFX);
        damageInstance = RuntimeManager.CreateInstance(damageSFX);


    }

    //Methods to make player controls work and to access it in the code
    public void OnEnable()
    {
        playerControls.Player.Enable();
    }

    public void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void Start()
    {
        //Get player state and set default values
        playerState = GetComponent<PlayerStateList>();
        SetGravityScale(Data.gravityScale);

        playerState.IsFacingRight = true;
        playerState.IsIdle = true;
        //canAerialCombo = true;
        isSleeping = false;

        Data.respawnPoint = this.transform.position;

        Data.maxHP = PersistentDataManager.Instance.MaxHP;
        Data.maxSP = PersistentDataManager.Instance.MaxSP;
        Data.canDoubleJump = PersistentDataManager.Instance.CanDoubleJump;
        Data.canWallJump = PersistentDataManager.Instance.CanWallJump;
        Data.canDash = PersistentDataManager.Instance.CanDash;
        Data.canScytheThrow = PersistentDataManager.Instance.CanScytheThrow;
        Data.canViewMap = PersistentDataManager.Instance.CanViewMap;

        LastJumpTime = 0;
        LastWallJumpTime = 0;

        walkModifier = 1;
        canSleepWalk = false;

        if (PersistentDataManager.Instance.FirstSpawn)
        {
            SpawnAtLastRestPoint();
            PersistentDataManager.Instance.ToggleFirstSpawn(false);
        }

        //TempResetData();
    }

    private void Update()
    {

        if (!isSleeping)
        {
            //Update all timers -----------------------------------
            //Collision Checks
            LastOnGroundTime -= Time.deltaTime;
            LastOnWallTime -= Time.deltaTime;
            LastHeldWallTime -= Time.deltaTime;
            LastOnWallRightTime -= Time.deltaTime;
            LastOnWallLeftTime -= Time.deltaTime;

            //Movement
            LastPressedJumpTime -= Time.deltaTime;
            LastPressedDashTime -= Time.deltaTime;

            if (playerState.IsJumping)
                LastJumpTime += Time.deltaTime;

            LastWallJumpTime += Time.deltaTime;

            //Movement values --------------------------------------
            //Movement/Walking input
            moveInput = playerControls.Player.Move.ReadValue<Vector2>();

            //Check direction of player, compare it to deadzone to make sure the player doesn't flick back and forth
            if (moveInput.x > Data.deadzone)
                CheckDirectionToFace(true);
            else if (moveInput.x < -Data.deadzone)
                CheckDirectionToFace(false);


            //Variable Updates ----------------------------------------------
            //Check if player hit ground or walls
            CollisionChecks();

            //Update movement variables
            UpdateJumpVariables();
            UpdateDashVariables();

            //Slide Check
            UpdateSlideVariables();

            //Gravity check
            UpdateGravityVariables();

          
        }


        else if (isSleeping && canSleepWalk)
        {
            //Movement values --------------------------------------
            //Movement/Walking input
            moveInput = playerControls.Player.Move.ReadValue<Vector2>();

            //Check direction of player, compare it to deadzone to make sure the player doesn't flick back and forth
            if (moveInput.x > Data.deadzone)
                CheckDirectionToFace(true);
            else if (moveInput.x < -Data.deadzone)
                CheckDirectionToFace(false);
        }

        //Check if the player hit the ground (outside of sleep so it can exit sleep)
        if (playerState.IsAttacking)
        {
            UpdateDownAttackVariables();
            moveInput.y = playerControls.Player.Move.ReadValue<Vector2>().y;
        }

        if (UIManager.Instance.fullMapUI != null)
        {
            if (zoomingMapIn)
                UIManager.Instance.ZoomMap(true);

            else if (zoomingMapOut)
                UIManager.Instance.ZoomMap(false);

            if (playerControls.Player.MapPan.ReadValue<Vector2>() != Vector2.zero && !movingOnMapOpen)
                UIManager.Instance.PanMap(playerControls.Player.MapPan.ReadValue<Vector2>(), false);

            else if (panningMap)
                UIManager.Instance.PanMap(playerControls.Player.MapPanDrag.ReadValue<Vector2>(), true);

            if (playerControls.Player.MapPan.ReadValue<Vector2>() == Vector2.zero && UIManager.Instance.fullMapUI.activeInHierarchy)
                movingOnMapOpen = false;
        }
    }

    private void FixedUpdate()
    {
        //Handle player walking, make sure the player doesn't walk while dashing
        if (!isSleeping)
        {
            //if (!playerState.IsDashing)// && !playerState.IsAttacking && !playerCombat.IsComboing)
            if (!playerState.IsDashing && CanExecuteWhileAttacking())
            {
                if (playerState.IsWallJumping)
                    Walk(Data.wallJumpRunLerp);
                else
                    Walk(1);
            }
        }
        else if (canSleepWalk)
        {
            Walk(1);
        }
            

        if (scytheThrown && !UIManager.Instance.scytheThrowInMenu)
            heldScytheSprite.SetActive(false);

        else
            heldScytheSprite.SetActive(true);

        if (hasInvincibility)
        {          
            SetSpriteColors(Color.red);
            invincibilityTimer -= Time.deltaTime;

            if (invincibilityTimer <= 0)
            {
                hasInvincibility = false;
                SetSpriteColors(Color.white);
            }
        }

        //Handle Slide
        if (playerState.IsSliding)
        {
            playSlideSFX(slideInstance);
            Slide();
        }

        // Grounded() check did not work here
        if (!playerState.IsJumping && !isJumpFalling && !playerState.IsAttacking && !playerState.IsDashing)
        {
            if (Math.Abs(rb.velocity.x) < 1)
            {
                PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.Idle);
            }
            else
            {
                PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.Run);
            }
        }

        CheckIdle();     
        if (playerState.IsIdle)
        {
            ResetPlayerOffset();         
        }
        
    }
    #endregion

    //Input Methods -----------------------------------------------------------------------------------------------
    #region Input Methods and Events

    //Jump Input
    private void OnJump(InputValue value)
    {
        if (isSleeping || Time.timeScale == 0)
            return;

        //Values to check if the key is down or up - will determine if the jump should be canceled or not
        //Key Down, continue jumping
        if (value.isPressed)
        {
            LastPressedJumpTime = Data.jumpInputBufferTime;
            LastJumpTime = 0;
        }
        //Key Up, cancel jumping
        else if (!value.isPressed)
        {
            if (CanJumpCancel() || (Data.canWallJumpCancel && CanWallJumpCancel()))
                isJumpCancel = true;
        }

        //Make sure the player is not dashing
        //Debug.Log(playerState.IsAttacking);
        if (!playerState.IsDashing && CanExecuteWhileAttacking() && value.isPressed)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                //Set states
                playerState.IsJumping = true;
                playerState.IsWallJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;

                //If mid attack, stop the combo
                if (playerCombat.AttackDurationTime > 0)
                    EndCombo();

                Jump();
            }
            //Wall Jump
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                //Set states
                playerState.IsWallJumping = true;
                playerState.IsJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;

                //Set timer and direction of wall jump
                wallJumpStartTime = Time.time;
                lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                //Stops player from double jumping off wall
                if (!Data.preserveDoubleJump)
                    airJumpCounter = Data.maxAirJumps;

                //If mid attack, stop the combo
                if (playerCombat.AttackDurationTime > 0)
                    EndCombo();

                WallJump(lastWallJumpDir);
            }
            //Double jump
            else if (CanDoubleJump())
            {
                //Set states
                playerState.IsJumping = true;
                playerState.IsWallJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;

                //Add to jump counter
                airJumpCounter++;

                //If mid attack, stop the combo
                if (playerCombat.AttackDurationTime > 0)
                    EndCombo();

                Jump();
            }
        }
    }

    //Dash Input
    private void OnDash()
    {
        if (isSleeping)
            return;

        LastPressedDashTime = Data.dashInputBufferTime;
    }

    private void OnPause()
    {
        UIManager.Instance.Pause();
    }

    public void OnCameraLook(InputValue value)
    {
        if (UIManager.Instance.pauseScript.isPaused || Time.timeScale == 0)
            return;

        //Debug.Log("Camera Look " + value.Get<Vector2>().y);
        if (value.Get<Vector2>().y > CameraManager.Instance.Deadzone)
        {
            CameraManager.Instance.LookUp();
        }
        else if (value.Get<Vector2>().y < -CameraManager.Instance.Deadzone)
        {
            CameraManager.Instance.LookDown();
        }
        else
        {
            CameraManager.Instance.CameraLookReset();
        }
    }

    private void OnInteract()
    {
        if (UIManager.Instance.pauseScript.isPaused)
            return;

        if (interactionPrompt.interactable != null)
        {
            interactionPrompt.interactable.PerformInteraction();
        }
    }

    private void OnMap()
    {
        if (playerControls.Player.Move.IsPressed())
            movingOnMapOpen = true;

        else
            movingOnMapOpen = false;

        UIManager.Instance.ToggleMap();
    }

    private void OnMapZoomIn(InputValue value)
    {
        if (value.isPressed)
            zoomingMapIn = true;

        else
            zoomingMapIn = false;
    }

    private void OnMapZoomOut(InputValue value)
    {
        if (value.isPressed)
            zoomingMapOut = true;

        else
            zoomingMapOut = false;
    }

    private void OnMapRecenter()
    {
        UIManager.Instance.ResetMap();
    }

    private void OnMapKey()
    {
        UIManager.Instance.ToggleMapKey();
    }

    private void OnAttack(InputValue value)
    {
        if (value.isPressed)
            panningMap = true;

        else
            panningMap = false;
    }

    private void OnCancel()
    {
        if (Time.timeScale == 1)
            return;

        StartCoroutine(UIManager.Instance.Cancel(0.1f));
    }

    private void OnAbility()
    {
        if (isSleeping || Time.timeScale == 0 || currentSP <= 0 || scytheThrown || !Data.canScytheThrow)
            return;

        ExecuteScytheThrow();
    }

    public void TakeDamage(float damageTaken)
    {
        currentHP -= damageTaken;
        invincibilityTimer = 2.0f;
        hasInvincibility = true;
        takeDamageSFX();

        CheckForDeath();
    }

    public void Respawn()
    {
        Time.timeScale = 1.0f;
        this.hasInvincibility = false;

        currentHP = Data.maxHP;
        currentSP = Data.maxSP;

        PersistentDataManager.Instance.ToggleFirstSpawn(false);

        if (!playerState.IsFacingRight)
            Turn();

        SpawnAtLastRestPoint();
    }

    //Calculate physics for attacks ---------------------------------------------
    public void ExecuteBasicAttack()
    {
        if (isSleeping)
            return;

        //Check to make sure the player does not hit on combo cooldown
        if (playerCombat.LastComboTime < 0 && Data.hasStallForce)
        {
            //Aerial attack sleep / hitstop 
            if (!Grounded())
            {
                if (playerCombat.CanAerialCombo)
                {
                    playerCombat.IsAerialCombo = true;
                    EndSleep();

                    if (playerCombat.AttackClickCounter < Data.comboTotal)
                        Sleep(Data.comboSleepTime);
                    else
                        Sleep(Data.comboSleepTime / 2);
                }
            }
            //Ground attack sleep
            else
            {
                if (playerCombat.AttackClickCounter == Data.comboTotal)
                {
                    EndSleep();
                    Sleep(Data.comboSleepTime / 2);
                }
                else// if (playerCombat.AttackClickCounter > 1)
                {
                    EndSleep();
                    Sleep(Data.comboSleepTime);
                }
            }
        }
    }

    //Calculate physics for attacks ---------------------------------------------
    public void ExecuteUpAttack(bool shouldGroundAttack)
    {
        if (isSleeping)
            return;

        //Aerial attack check
        //if (Data.hasStallForce)
        //{       
            if (Grounded())
            {
                Sleep(Data.gUpAttackDuration);
            }
            else
            {
                Sleep(Data.aUpAttackDuration);
            }
        //}
    }

    //Calculate physics for attacks ---------------------------------------------
    public void ExecuteDownAttack(bool shouldGroundAttack)
    {
        if (isSleeping)
            return;

        //Aerial attack check
        //if (Data.hasStallForce)
        //{
            if (shouldGroundAttack)
            {
                //Sleep(Data.gDownAttackDuration);
            }
            else
            {
                Sleep(Data.aDownAttackDuration);
            }
        //}
    }

    public void ExecuteScytheThrow()
    {
        GameObject scythe = Instantiate(scytheProjectilePrefab, this.transform.position, Quaternion.identity);
        currentSP -= 5;
        scytheThrown = true;
    }
    #endregion

    //Movement Method Calculations --------------------------------------------------------------------------------
    #region Movement Calculations

    //Walking
    private async void Walk(float lerpAmount)
    {
        if (isSleeping && !canSleepWalk)
            return;
        //Get direction and normalize it to either 1 or -1 
        int direction = Math.Sign(XInputDirection());


        if (direction == 0)
            playerState.IsWalking = false;
        else
            playerState.IsWalking = true;


        //Calculate the direction and our desired velocity
        float targetSpeed = direction * Data.walkMaxSpeed * walkModifier;
        //float targetSpeed = moveInput.x * Data.walkMaxSpeed; <---------- used for walking at a slower pace
        //Smooth changes to direction and speed using a lerp function
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        //Calculate acceleration rate
        float accelRate;
        //Gets an acceleration value based on if we are accelerating or decelerating 
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.walkAccelAmount : Data.walkDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.walkAccelAmount * Data.accelInAir : Data.walkDeccelAmount * Data.deccelInAir;

        //Bonus jump acceleration
        //Increase acceleration and maxSpeed when at the apex of the player's jump - makes it feel more bouncy/responsive
        if ((playerState.IsJumping || playerState.IsWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        if (direction != 0)
        {
            float cameraOffset = Data.cameraWalkOffset * direction;
            PlayWalkSFX();
            CameraManager.Instance.StartScreenXOffset(cameraOffset, 0.2f,2);
        }
    }

    //Used for player direction
    private void Turn()
    {
        if (Time.timeScale == 0)
            return;

        //Transform local scale of object
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        playerState.IsFacingRight = !playerState.IsFacingRight;

        //Updates scale of UI so that it is always facing right
        Vector3 tempScale = interactionPrompt.gameObject.GetComponentInChildren<Canvas>().transform.localScale;
        tempScale.x *= -1;
        interactionPrompt.gameObject.GetComponentInChildren<Canvas>().transform.localScale = tempScale;
    }

    //Jump
    private void Jump()
    {
        //Ensures we can't call jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        //Offset force 
        float force = Data.jumpForce;
        force = OffsetYForce(force);

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        jumpVelocity = rb.velocity.y;

        playJumpSFX(jumpInstance, 0);
        FMODJumpFinished = false;
        FMODIsLandedPlayed = false;

        PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.Jump);
    }

    //Wall jump
    private void WallJump(int dir)
    {
        //Ensures we can't call wall jump multiple times from one press
        LastPressedJumpTime = 0;
        LastWallJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        //Perform Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall
        force = OffsetForce(force);

        //Use impulse to apply force instantly ignoring mass
        rb.AddForce(force, ForceMode2D.Impulse);

        if (Data.doTurnOnWallJump)
        {
            if (dir > 0 && !playerState.IsFacingRight)
                Turn();
            else if (dir < 0 && playerState.IsFacingRight)
                Turn();
        }
        playJumpSFX(jumpInstance, 1);

    }

    //Slide
    private void Slide()
    {
        //Remove the remaining upwards Impulse to prevent upwards sliding
        if (rb.velocity.y >= 0)
        {
            rb.AddForce(-rb.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        //Slide
        float movement = Data.slideSpeed * Data.slideAccel;
        movement = OffsetYForce(movement);

        if(rb.velocity.y < movement)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, movement, 0));
        }
        else
        {
            rb.AddForce(movement * Vector2.up);
        }

        
    }

    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        //Dash timers
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        //Start time of the dash
        float startTime = Time.time;

        //Update states
        dashesLeft--;
        isDashAttacking = true;

        //Become invincible and make sprite transparent while dashing
        hasDashInvincibility = true;
        Color tmp = animator.GetComponent<SpriteRenderer>().color;
        tmp.a = 0.5f;
        animator.GetComponent<SpriteRenderer>().color = tmp;

        //Update gravity and sleep other movements to make dash feel more juicy
        SetGravityScale(0);
        rb.velocity = Vector2.zero;
        Sleep(0.1f);

        yield return new WaitForSecondsRealtime(Data.dashSleepTime);             

        //Get direction to dash in
        int direction = XInputDirection();
        //If player is not moving / doesn't have direction, dash in the most recent input
        if (direction == 0)
        {
            if (playerState.IsFacingRight)
                direction = 1;
            else
                direction = -1;
        }
       
        rb.AddForce(Vector2.right * direction * Data.dashSpeed, ForceMode2D.Impulse);

        playDashSFX();
        
        //Update camera
        float cameraOffset = Data.cameraDashOffset * direction;
        CameraManager.Instance.StartScreenXOffset(cameraOffset, Data.dashAttackTime / 2, 3);

        yield return new WaitForSecondsRealtime(Data.dashAttackTime);

        //Stop dash
        isDashAttacking = false;

        //Reset movement back to close to the walking speed
        SetGravityScale(Data.gravityScale);
        rb.velocity = new Vector2(Data.dashEndSpeed.x * direction, 0);

        yield return new WaitForSecondsRealtime(Data.dashEndTime);

        float endTime = Time.time;
        //Debug.Log("Total Dash Time: " + (endTime - startTime));

        //Dash over
        playerState.IsDashing = false;
        hasDashInvincibility = false;
        tmp = animator.GetComponent<SpriteRenderer>().color;
        tmp.a = 1f;
        animator.GetComponent<SpriteRenderer>().color = tmp;
        //Debug.Log("Current Transparency2: " + animator_T.GetComponent<SpriteRenderer>().color.a);
    }

    //Delay period between dashes
    private IEnumerator RefillDash(int amount)
    {
        dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        dashRefilling = false;
        dashesLeft = Mathf.Min(Data.dashAmount, dashesLeft + 1);
    }

    private void BasicAttack()
    {
        int direction;         
        if (playerState.IsFacingRight)
            direction = 1;
        else
            direction = -1;

        Debug.Log("Basic Attack");

        rb.velocity = new Vector2(rb.velocity.x * .1f, 0);

        if (playerCombat.IsAerialCombo)
            rb.AddForce(new Vector2(direction, 0) * Data.comboAerialPForce, ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(direction, 0) * Data.comboGroundPForce, ForceMode2D.Impulse);
    }

    private void UpAttack()
    {
        SetGravityScale(1);
        //rb.AddForce(Vector2.down * Data.groundUpwardPForce, ForceMode2D.Impulse);
        rb.velocity = new Vector2(0, Data.groundUpwardPForce);
    }

    private void DownAttack()
    {
        SetGravityScale(1);
        //rb.AddForce(Vector2.down * Data.aerialDownwardPForce, ForceMode2D.Impulse);
        rb.velocity = new Vector2(0, -Data.aerialDownwardPForce);
    }

    #endregion

    //Methods to update movement variables ------------------------------------------------------------------------
    #region Movement Variable Updates

    //Variables used for every type of jump
    private void UpdateJumpVariables()
    {
        if (playerState.IsJumping && rb.velocity.y < 0.1)
        {
            playerState.IsJumping = false;

            isJumpFalling = true;

            LastJumpTime = 0;

            FMODJumpFinished = true;

        }

        if (playerState.IsWallJumping && Time.time - wallJumpStartTime > Data.wallJumpTime)
        {
            playerState.IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !playerState.IsJumping && !playerState.IsWallJumping)
        {
            isJumpCancel = false;

            isJumpFalling = false;

        }

        if (Grounded())
        {
            airJumpCounter = 0;

            if (FMODIsLandedPlayed == false && FMODJumpFinished == true)
            {
                stopSlideSFX(slideInstance);
                playLandSFX(landInstance);
                FMODIsLandedPlayed = true;
            }
            


        }
        else if (Data.resetJumpOnWall && OnWall())
        {
            //Debug.Log("Reseting wall jump");
            airJumpCounter = 0;
        }

        // Update animator jump variable
        animator.SetBool("IsJumping", playerState.IsJumping || isJumpFalling);

    }

    //Dash variables
    private void UpdateDashVariables()
    {
        if (CanDash() && LastPressedDashTime > 0)
        {
            //If not direction pressed, dash forward
            if (moveInput != Vector2.zero)
                lastDashDir = moveInput;
            else
                lastDashDir = playerState.IsFacingRight ? Vector2.right : Vector2.left;

            //If mid attack, stop the combo
            if (playerCombat.IsComboing)
                EndCombo();      

            //Set states
            playerState.IsDashing = true;
            playerState.IsJumping = false;
            playerState.IsWallJumping = false;
            isJumpCancel = false;

            StartCoroutine(nameof(StartDash), lastDashDir);

            PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.Dash);
        }
    }

    //Variables for wall sliding
    private void UpdateSlideVariables()
    {
        if (Data.mustHoldWallToJump)
        {
            if (CanSlide() && ((LastOnWallLeftTime > 0 && moveInput.x < -Data.deadzone) || (LastOnWallRightTime > 0 && moveInput.x > Data.deadzone)))
            {
                playerState.IsSliding = true;
            }
                
            else
            {
                playerState.IsSliding = false;
                LastHeldWallTime = Data.coyoteTime;
            }
        }
        else
        {
            if (CanSlide() && ((LastOnWallLeftTime > 0 && moveInput.x < Data.deadzone) || (LastOnWallRightTime > 0 && moveInput.x > -Data.deadzone)))
            {
                playerState.IsSliding = true;
            }
            else
                playerState.IsSliding = false;
        }
    }

    //Updates the gravity based on the state of the player
    private void UpdateGravityVariables()
    {
        if (!playerState.IsDashing)
        {
            //Higher gravity if we've released the jump input or are falling
            if (playerCombat.IsAerialCombo && playerCombat.AttackDurationTime > 0)
            {
                SetGravityScale(0);
            }
            //No gravity if the player is sliding
            else if (playerState.IsSliding)
            {
                SetGravityScale(0);

                if ((XInputDirection() == -1 && playerState.IsFacingRight) || (XInputDirection() == 1 && !playerState.IsFacingRight))
                {
                    playerState.IsSliding = false;
                    SetGravityScale(1);
                }                   
            }
            else if (rb.velocity.y < 0 && moveInput.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (isJumpCancel)
            {
                //Higher gravity if jump button released
                float jumpRatio = rb.velocity.y / jumpVelocity;
                float jumpCancelRatio = Data.jumpCancelGravityMult * jumpRatio;
                float jumpCancelScale = Mathf.Clamp(Data.jumpCancelGravityMult * jumpCancelRatio, Data.fastFallGravityMult, Data.jumpCancelGravityMult);
                SetGravityScale(Data.gravityScale * jumpCancelScale);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
            }
            else if ((playerState.IsJumping || playerState.IsWallJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (rb.velocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            SetGravityScale(0);
        }
    }

    private void UpdateDownAttackVariables()
    {
        //End Down Attack   
        if (Grounded() && playerCombat.CurrentAttackDirection == PlayerCombat.AttackDirection.Down && playerCombat.IsAerialAttacking)
        {
            EndSleep();
            playerCombat.CurrentAttackDirection = PlayerCombat.AttackDirection.Empty;
            playerCombat.AttackDurationTime = Data.aDownAttackReset;
            Sleep(playerCombat.AttackDurationTime);
        }
    }
    #endregion

    //Status Checkers ---------------------------------------------------------------------------------------------
    #region Statuses & Checkers
    //Check collisions on every side of the player
    private void CollisionChecks()
    {
        if (!playerState.IsDashing && !playerState.IsJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer)) //check box overlap with ground
            {
                LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
            }


            //Right Wall Check
            if ((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight) && !playerState.IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;
            

            //Left Wall Check
            //Debug.Log((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight) && !playerState.IsWallJumping);
            if ((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight) && !playerState.IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
    }

    //Check ground specific collision and return a bool
    public bool Grounded()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
    }

    //Check wall specific collision and return a bool
    private bool OnWall()
    {
        return (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight)
                    || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight)) && !playerState.IsWallJumping)
                    || (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight)) && !playerState.IsWallJumping);
    }

    //Check if the player is not doing anything and set the state to idle
    private void CheckIdle()
    {
        if (!playerState.IsJumping && !playerState.IsWallJumping && !playerState.IsDashing && !playerState.IsSliding && !playerState.IsWalking && !playerState.IsAttacking)
            playerState.IsIdle = true;
        else
            playerState.IsIdle = false;
    }

    private void ResetPlayerOffset()
    {
        float dir = (playerState.IsFacingRight) ? 1 : -1;
        float cameraOffset = Data.cameraOffset * dir;
        CameraManager.Instance.StartScreenXOffset(cameraOffset, 0.2f,1);
    }

    //Check direction that the player should face and execute it 
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != playerState.IsFacingRight)
        {
            //Don't turn if player is mid combo
            if (playerCombat.IsComboing && !Data.canTurnDuringCombo)
            {
                return;
            }

            //Check for wall jumping and where it automatically turns the player
            if (Data.doTurnOnWallJump)
            {
                //Buffer to make sure turning looks smooth
                if (Data.wallTurnBuffer < LastWallJumpTime)
                    Turn();
            }
            //Default, just turn
            else
                Turn();
        }
    }

    //Checks for jump states
    private bool CanJump()
    {
        return Data.canJump && LastOnGroundTime > 0 && !playerState.IsJumping && CanBreakCombo();
    }

    private bool CanWallJump()
    {
        if (Data.canWallJump && CanBreakCombo())
        {
            if (Data.mustHoldWallToJump)
            {
                return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && ((!playerState.IsWallJumping && XInputDirection() != 0) ||
                    //Left 
                    (LastOnWallRightTime > 0 && lastWallJumpDir == 1 && (XInputDirection() == -1 || LastHeldWallTime > 0)) ||
                    //Right
                    (LastOnWallLeftTime > 0 && lastWallJumpDir == -1 && (XInputDirection() == 1 || LastHeldWallTime > 0)));
            }
            else
            {
                return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!playerState.IsWallJumping ||
                    //Left 
                    (LastOnWallRightTime > 0 && lastWallJumpDir == 1) ||
                    //Right
                    (LastOnWallLeftTime > 0 && lastWallJumpDir == -1));
            }
        }
        else
            return false;
    }

    private bool CanDoubleJump()
    {
        return Data.canDoubleJump && Data.canDoubleJump && airJumpCounter < Data.maxAirJumps && !Grounded() && CanBreakCombo();
    }

    //Checks for jump cancel
    private bool CanJumpCancel()
    {
        return playerState.IsJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCancel()
    {
        return playerState.IsWallJumping && rb.velocity.y > 0;
    }

    //Check for Dash state
    private bool CanDash()
    {
        if (Data.canDash)// && CanBreakCombo())
        {
            if (!playerState.IsDashing && dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !dashRefilling) //(LastOnGroundTime > 0 || OnWall())
            {
                StartCoroutine(nameof(RefillDash), 1);
            }

            return dashesLeft > 0;
        }
        else
            return false;
    }

    //Check if the player is able to break combo after enough time
    private bool CanBreakCombo(float breakMult = 2 / 3f)
    {
        //Debug.Log(Data.attackBufferTime);
        return playerCombat.AttackDurationTime < Data.attackBufferTime * breakMult;
    }

    //Check to see if the player is on the wall and is sliding
    public bool CanSlide()
    {
        if (Data.canSlide)
        {
            if (LastOnWallTime > 0 && !playerState.IsJumping && !playerState.IsWallJumping && !playerState.IsDashing && LastOnGroundTime <= 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    //Checks to see if the player can execute a method when attacking due to the stall force
    private bool CanExecuteWhileAttacking()
    {
        if (Data.hasStallForce)
            return !playerState.IsAttacking && !playerCombat.IsComboing;
        else
            return true;
    }

    //Check health for death
    private void CheckForDeath()
    {
        if (currentHP <= 0)
        {
            UIManager.Instance.HandlePlayerDeath();
            foreach (Room room in PersistentDataManager.Instance.rooms)
            {
                if (room.gameObject.activeInHierarchy)
                    room.GetComponent<EnemyManager>().DeleteEnemies();


                if (room.GetComponentInChildren<ArenaManager>() != null)
                {
                    room.GetComponentInChildren<ArenaManager>().CombatEnd();
                }
            }
        }

        else
        {
            UIManager.Instance.DamageVignette();
            CameraShake.Instance.ShakeCamera(5, 4, .25f);
        }
    }
    #endregion

    //Helper Methods ----------------------------------------------------------------------------------------------
    #region Helper Methods
    //Set colors of the sprite, along with the transparency
    private void SetSpriteColors(Color color, float transparency = 1)
    {
        color.a = transparency;
        animator.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    //Set x direction to -1 or 1, even if using analog stick
    private int XInputDirection()
    {
        //Added deadzone to account for controller drift
        if (moveInput.x < -Data.deadzone)
            return -1;
        else if (moveInput.x > Data.deadzone)
            return 1;
        else
            return 0;
    }

    //Set y direction to -1 or 1, even if using analog stick
    public int YInputDirection()
    {
        //Deadzone to account for controller drift
        if (moveInput.y < -Data.deadzone)
            return -1;
        else if (moveInput.y > Data.deadzone)
            return 1;
        else
            return 0;
    }

    //End combo and update the associated states
    private void EndCombo()
    {
        //Debug.Log("Ending Combo");
        playerCombat.ResetCombo();
        playerState.IsAttacking = false;
        EndSleep();
    }

    //Set gravity 
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    //Offset force to make sure there isn't extra force added to specific movements
    private Vector2 OffsetForce(Vector2 force)
    {
        return force -= rb.velocity;
    }

    private float OffsetYForce(float force)
    {
        return force -= rb.velocity.y;
    }

    private float OffsetXForce(float force)
    {
        return force -= rb.velocity.x;
    }

    //Sleep for delaying movement
    public void Sleep(float duration)
    {
        //Method to help delay time for movement
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private void Sleep()
    {
        SetGravityScale(0);
        isSleeping = true;
    }

    public void ToggleSleep(bool toggleOn)
    {
        if (toggleOn)
        {
            rb.velocity = Vector2.zero;
            isSleeping = true;
        }

        else
            isSleeping = false;
        
    }

    public void SleepWalk()
    {
        rb.velocity = Vector2.zero;
        isSleeping = true;
        canSleepWalk = true;
    }

    public void EndSleepWalk()
    {
        isSleeping = false;
        canSleepWalk = false;
    }

    public void EndSleep()
    {
        //Method to stop the coroutine from running 
        StopCoroutine(nameof(PerformSleep));
        SetGravityScale(1);
        //rb.velocity = Vector2.zero;
        isSleeping = false;
        canSleepWalk = false;
    }

    private IEnumerator PerformSleep(float duration)
    {
        //Debug.Log("Performing sleep, duration: " + duration);
        //Sleeping
        SetGravityScale(0);
        isSleeping = true;
        rb.velocity = Vector2.zero;

        //Combat force calculations       
        if (playerState.IsAttacking)
        {
            switch (playerCombat.CurrentAttackDirection)
            {
                case PlayerCombat.AttackDirection.Up:
                    UpAttack();
                    break;
                case PlayerCombat.AttackDirection.Down:
                    DownAttack();
                    break;
                case PlayerCombat.AttackDirection.Side:
                    BasicAttack();
                    break;
            }
        }

        yield return new WaitForSecondsRealtime(duration / 8);

        //Reset impulse from combat
        //if (playerCombat.IsAerialCombo)
        //rb.velocity = new Vector2(rb.velocity.x * .05f, 0);

        if (playerState.IsAttacking)
        {
            switch (playerCombat.CurrentAttackDirection)
            {
                case PlayerCombat.AttackDirection.Up:
                    rb.velocity = new Vector2(0, 0);
                    break;
                case PlayerCombat.AttackDirection.Down:
                    rb.velocity = new Vector2(0, 0);
                    break;
                case PlayerCombat.AttackDirection.Side:
                    rb.velocity = new Vector2(rb.velocity.x * .05f, 0);
                    break;
            }
        }
            
        yield return new WaitForSecondsRealtime(duration / 8 * 7);

        SetGravityScale(1); 
        isSleeping = false;
    }

    //Wall collision check gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
    }

    private void SpawnAtLastRestPoint()
    {
        Debug.Log("Spawning at Rest Point");

        PersistentDataManager.Instance.LoadRoom();

        Vector3 newSpawn = PersistentDataManager.Instance.SpawnPoint;

        this.gameObject.transform.position = newSpawn;
        Data.respawnPoint = newSpawn;

        if (!PersistentDataManager.Instance.FirstTimeInDenial)
            currentHP = Data.maxHP;
    }

    private void PlayWalkSFX()
    {
        float timeDifference = Time.time - currentTime;
        if(timeDifference > 1/footstepSpeed && Grounded())
        {
            FMODUnity.RuntimeManager.PlayOneShot(walkSFX);
            currentTime = Time.time;
        }
    }

    private void playJumpSFX(EventInstance jumpInstance, int jumpStatus)
    {
        stopSlideSFX(slideInstance);
        jumpInstance.start();
        if (jumpStatus == 1) {
            slideInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            wallLeapInstance.start();
            }
    }
    
    private void playDashSFX()
    {
        stopSlideSFX(slideInstance);
        FMODUnity.RuntimeManager.PlayOneShot(dashSfx);
    }

    private void takeDamageSFX()
    {
        damageInstance.start();
    }

    private void playLandSFX(EventInstance landInstance)
    {
        landInstance.start();
    }

    private void playSlideSFX(EventInstance slideInstance)
    {
        if(isSlidingPlayed == false)
        {
            isSlidingPlayed = true;
            slideInstance.start();
            slideInstance.setParameterByName("SlideStatus", 0);
        }

    }

    private void stopSlideSFX(EventInstance slideInstance)
    {
        slideInstance.setParameterByName("SlideStatus", 1);
        isSlidingPlayed = false;
        //Debug.Log("Stopped Sliding");
    }
        #endregion

        private void TempResetData()
        {
            Data.canDash = true;
            Data.canDoubleJump = true;
            Data.canWallJump = true;
        }
    }