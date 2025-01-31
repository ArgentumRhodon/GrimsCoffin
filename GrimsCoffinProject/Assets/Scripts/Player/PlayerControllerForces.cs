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

public class PlayerControllerForces : MonoBehaviour
{
    //Scriptable object which holds all data on player movement parameters
    public PlayerData Data;

    //Rigidbody and player state
    public Rigidbody2D rb { get; private set; }
    private PlayerStateList playerState;

    //Timers
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastHeldWallTime { get; private set; } 
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastJumpTime { get; private set; }
    public float LastWallJumpTime { get; private set; }

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

    //Input parameters
    private Vector2 moveInput;

    // Animation Stuff
    [SerializeField] private Animator animator;
    [SerializeField] private Animator scytheAnimator; // Top

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }

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

    [Header("Player UI")]
    [SerializeField] public InteractionPrompt interactionPrompt;

    //Singleton so the controller can be referenced across scripts
    public static PlayerControllerForces Instance;

    //Reference to Player Input/Controls
    private PlayerControls playerControls;
    private PlayerCombat playerCombat;

    //Time Variables
    private float localDeltaTime;
    private bool isSleeping;


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

        LastJumpTime = 0;
        LastWallJumpTime = 0;

        if (PersistentDataManager.Instance.FirstSpawn)
        {
            SpawnAtLastRestPoint();
            PersistentDataManager.Instance.ToggleFirstSpawn(false);
        }
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
    }

    private void FixedUpdate()
    {
        //End sleep if moving after combo
        if (isSleeping)
            if (moveInput.x != 0)
                if (playerCombat.ShouldResetCombo())
                    EndSleep();

        //Handle player walking, make sure the player doesn't walk while dashing
        if (!isSleeping)
        {
            if (!playerState.IsDashing && !playerState.IsAttacking)
            {
                if (playerState.IsWallJumping)
                    Walk(Data.wallJumpRunLerp);
                else
                    Walk(1);
            }
            if(playerState.IsAttacking && Grounded())
            {
                Walk(1);
            }
        }

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
            Slide();

        animator.SetFloat("xVel", Mathf.Abs(rb.velocity.x));

        CheckIdle();     
        if (playerState.IsIdle)
        {
            ResetPlayerOffset();         
        }
            
    }

    private void SetSpriteColors(Color color, float transparency = 1)
    {
        color.a = transparency;
        animator.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    //Input Methods ----------------------------------------------------------------------------------------------
    //Jump Input
    private void OnJump(InputValue value)
    {
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
        if (!playerState.IsDashing && !playerState.IsAttacking && value.isPressed)
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
            EndSleep();
        LastPressedDashTime = Data.dashInputBufferTime;
    }

    private void OnPause()
    {
        UIManager.Instance.Pause();
    }

    public void StartAttack()
    {
        if (playerState.IsDashing)
            return;

        //Do not hit during combo
        if (playerCombat.LastComboTime < 0) 
        {
            //Aerial attack sleep / hitstop 
            if (!Grounded())
            {
                if (playerCombat.CanAerialCombo)
                {
                    playerCombat.IsAerialCombo = true;
                    EndSleep();

                    if (playerCombat.AttackClickCounter < Data.comboTotal)
                        Sleep(Data.comboAerialTime);
                    else
                        Sleep(Data.comboAerialTime/2);
                }
            }
            else
            {
                //Potential ground combat hitstop - NEEDS FIXING
/*                if (playerCombat.AttackCounter == Data.comboTotal)
                    Sleep(Data.comboSleepTime / 2);
                else if (playerCombat.AttackCounter > 2)
                {
                    EndSleep();
                    Sleep(Data.comboSleepTime);
                }*/
            }
        }
    }

    public void OnCameraLook(InputValue value)
    {
        if (UIManager.Instance.pauseScript.isPaused)
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
            CameraManager.Instance.Reset();
        }
    }

    public void TakeDamage(float damageTaken)
    {
        currentHP -= damageTaken;
        invincibilityTimer = 2.0f;
        hasInvincibility = true;

        UIManager.Instance.DamageVignette();
        CameraShake.Instance.ShakeCamera(5, 4, .25f);

        CheckForDeath();
    }

    private void CheckForDeath()
    {
        if (currentHP <= 0)
        {
            UIManager.Instance.HandlePlayerDeath();
        }
    }

    public void Respawn()
    {
        Time.timeScale = 1.0f;
        this.hasInvincibility = false;

        currentHP = Data.maxHP;
        currentSP = Data.maxSP;

        if (Data.respawnPoint != null)
            this.gameObject.transform.position = Data.respawnPoint;
    }

    //Movement Method Calculations ----------------------------------------------------------------------------------------------
    //Walking
    private void Walk(float lerpAmount)
    {
        //Get direction and normalize it to either 1 or -1 
        int direction = XInputDirection();
        if (direction != 0)
        {
            if (direction > 0)
                direction = 1;
            else
                direction = -1;
        }

        if (direction == 0)
            playerState.IsWalking = false;
        else
            playerState.IsWalking = true;


        //Calculate the direction and our desired velocity
        float targetSpeed = direction * Data.walkMaxSpeed;
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

    //Methods to update movement variables ------------------------------------------------------------------------
    private void UpdateJumpVariables()
    {
        if (playerState.IsJumping && rb.velocity.y < 0)
        {
            playerState.IsJumping = false;

            isJumpFalling = true;

            LastJumpTime = 0;
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
        }
        else if (Data.resetJumpOnWall && OnWall())
        {
            //Debug.Log("Reseting wall jump");
            airJumpCounter = 0;
        }
    }

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
            if (playerCombat.AttackDurationTime > 0)
                EndCombo();      

            //Set states
            playerState.IsDashing = true;
            playerState.IsJumping = false;
            playerState.IsWallJumping = false;
            isJumpCancel = false;

            StartCoroutine(nameof(StartDash), lastDashDir);
        }
    }

    private void UpdateSlideVariables()
    {
        if (Data.mustHoldWallToJump)
        {
            if (CanSlide() && ((LastOnWallLeftTime > 0 && moveInput.x < -Data.deadzone) || (LastOnWallRightTime > 0 && moveInput.x > Data.deadzone)))
                playerState.IsSliding = true;
            else
            {
                playerState.IsSliding = false;
                LastHeldWallTime = Data.coyoteTime;
            }
        }
        else
        {
            if (CanSlide() && ((LastOnWallLeftTime > 0 && moveInput.x < Data.deadzone) || (LastOnWallRightTime > 0 && moveInput.x > -Data.deadzone)))
                playerState.IsSliding = true;
            else
                playerState.IsSliding = false;
        }
    }

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

    //Helper Methods ----------------------------------------------------------------------------------------------
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
    private bool Grounded()
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

    private void CheckIdle()
    {
        if(!playerState.IsJumping && !playerState.IsWallJumping && !playerState.IsDashing && !playerState.IsSliding && !playerState.IsWalking && !playerState.IsAttacking)
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
        if (Data.canDash && CanBreakCombo())
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

    private bool CanBreakCombo(float breakMult = 2/3f)
    {
        //Debug.Log(Data.attackBufferTime);
        return playerCombat.AttackDurationTime < Data.attackBufferTime * breakMult;
    }

    private void EndCombo()
    {
        //Debug.Log("Ending Combo");
        playerCombat.ResetCombo();
        playerState.IsAttacking = false;
        EndSleep();
    }

    //Check to see if the player is on the wall and is sliding
    public bool CanSlide()
    {
        if(Data.canSlide)
        {
            if (LastOnWallTime > 0 && !playerState.IsJumping && !playerState.IsWallJumping && !playerState.IsDashing && LastOnGroundTime <= 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    //Set gravity 
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

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

    private void EndSleep()
    {
        //Method to stop the coroutine from running 
        StopCoroutine(nameof(PerformSleep));
        SetGravityScale(1);
        //rb.velocity = Vector2.zero;
        isSleeping = false;
    }

    private IEnumerator PerformSleep(float duration)
    {
        //Debug.Log("Performing sleep, duration: " + duration);
        //Sleeping
        SetGravityScale(0);
        isSleeping = true;

        //Combat force calculations
        if (playerCombat.IsAerialCombo)
        {
            int direction = XInputDirection();
            if (direction == 0)
            {
                if (playerState.IsFacingRight)
                    direction = 1;
                else
                    direction = -1;
            }


            rb.velocity = new Vector2(rb.velocity.x * .1f, 0);
            rb.AddForce(new Vector2(direction, 0) * Data.comboAerialPForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSecondsRealtime(duration / 8);

        //Reset impulse from combat
        if (playerCombat.IsAerialCombo)
            rb.velocity = new Vector2(rb.velocity.x * .05f, 0);

        yield return new WaitForSecondsRealtime(duration / 8 * 7);
        //Time.timeScale = 1;
 
        SetGravityScale(1);
        isSleeping = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
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
        UIManager.Instance.ToggleMap();
    }

    private void SpawnAtLastRestPoint()
    {
        Debug.Log("Spawning at Rest Point");
        Vector3 newSpawn = new Vector3(
            PlayerPrefs.GetFloat("XSpawnPos"),
            PlayerPrefs.GetFloat("YSpawnPos"),
            0);

        this.gameObject.transform.position = newSpawn;
        Data.respawnPoint = newSpawn;
        
        if (!PersistentDataManager.Instance.FirstTimeInDenial)
            currentHP = Data.maxHP;
    }
}
