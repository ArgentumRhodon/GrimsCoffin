using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.GoogleVr;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;
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
    [SerializeField] private Animator animator_T; // Top
    [SerializeField] private Animator animator_B; // Bottom

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
    [SerializeField] public Vector2 respawnPoint;

    [Header("Player Stats")]
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public float maxSP;
    [SerializeField] public float currentSP;
    [SerializeField] public float invincibilityTimer;
    [SerializeField] public bool hasInvincibility;

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
        //canAerialCombo = true;
        isSleeping = false;

        respawnPoint = this.transform.position;

        LastJumpTime = 0;
        LastWallJumpTime = 0;
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
                if (playerCombat.LastAttackTime < 0)
                    EndSleep();

        //Handle player walking, make sure the player doesn't walk while dashing
        if (!isSleeping)
        {
            if (!playerState.IsDashing)
            {
                if (playerState.IsWallJumping)
                    Walk(Data.wallJumpRunLerp);
                else
                    Walk(1);
            }
            else if (isDashAttacking)
            {
                Walk(Data.dashEndRunLerp);
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

        //Attack Check
        UpdateAttackVariables();

        animator_T.SetFloat("xVel", Mathf.Abs(rb.velocity.x));
        animator_B.SetFloat("xVel", Mathf.Abs(rb.velocity.x));
    }

    private void SetSpriteColors(Color color)
    {
        animator_T.gameObject.GetComponent<SpriteRenderer>().color = color;
        animator_B.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    //Input Methods ----------------------------------------------------------------------------------------------
    //Jump Input
    private void OnJump(InputValue value)
    {
        /*if (isSleeping)
            EndSleep();*/

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
        if (!playerState.IsDashing && value.isPressed)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                //Set states
                playerState.IsJumping = true;
                playerState.IsWallJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;

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
                if (!Data.resetJumpOnWall) 
                    airJumpCounter = Data.maxAirJumps;

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

    private void OnAttack()
    {
        if (playerState.IsDashing)
            return;

        //Do not hit during combo
        if (playerCombat.LastComboTime < 0) {
            //Combo attack counter
            playerCombat.AttackCounter++;
            playerCombat.LastAttackTime = Data.attackBufferTime;

            //Debug.Log(playerCombat.AttackCounter);

            animator_T.SetFloat("comboRatio", playerCombat.AttackCounter / 4f);
            animator_T.SetFloat("comboRatio", playerCombat.AttackCounter / 4f);
            animator_T.SetTrigger("Attack");
            animator_B.SetTrigger("Attack");

            //Aerial attack
            if (!Grounded())
            {
                if (playerCombat.CanAerialCombo)
                {
                    //Debug.Log(playerCombat.AttackCounter > 1);
                    playerCombat.IsAerialCombo = true;
                    EndSleep();
                    Sleep(Data.comboSleepTime);
                }
            }

            //Reset combo attack counter after a time delay
            if (playerCombat.AttackCounter >= 4)
            {
                playerCombat.LastComboTime = Data.comboSleepTime;
                playerCombat.AttackCounter = 0;
            }
        }
    }

    public void OnCameraLook(InputValue value)
    {
        Debug.Log("Camera Look " + value);
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

        currentHP = maxHP;
        currentSP = maxSP;

        if (respawnPoint != null)
            this.gameObject.transform.position = respawnPoint;
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


        //Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening/conserve momentum
            accelRate = 0;
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

/*        //If you're not conserving momentum, make sure the movement doesn't add 
        if (!Data.doConserveMomentum)
            if (Mathf.Abs(movement) < Mathf.Abs(rb.velocity.x))
                return;*/
        //Convert movement to a vector and apply it
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
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
        force = OffsetForce(force);

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
        /*float speedDif = Data.slideSpeed - rb.velocity.y;
        float movement = speedDif * Data.slideAccel; */
        float movement = Data.slideSpeed * Data.slideAccel;
        movement = OffsetForce(movement);

        //Clamp the movement here to prevent any over corrections
        //movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

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

        //Update gravity and sleep other movements to make dash feel more juicy
        SetGravityScale(0);
        Sleep(0.1f);

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

        //We keep the player's velocity at dash speed
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            rb.velocity = new Vector2(direction * Data.dashSpeed, 0);
            //Pauses the loop until the next frame
            yield return null;
        }

        startTime = Time.time;
        isDashAttacking = false;

        //Reset movement back to close to the walking speed
        SetGravityScale(Data.gravityScale);
        rb.velocity = new Vector2(Data.dashEndSpeed.x * direction, 0);

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }

        //Dash over
        playerState.IsDashing = false;
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
            Debug.Log("Reseting wall jump");
            airJumpCounter = 0;
        }
    }

    private void UpdateDashVariables()
    {
        if (CanDash() && LastPressedDashTime > 0)
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            Sleep(Data.dashSleepTime);

            //If not direction pressed, dash forward
            if (moveInput != Vector2.zero)
                lastDashDir = moveInput;
            else
                lastDashDir = playerState.IsFacingRight ? Vector2.right : Vector2.left;

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

    private void UpdateAttackVariables()
    {
        if (playerCombat.IsAerialCombo)
        {
            if (playerCombat.LastAttackTime < 0)
            {
                playerCombat.LastComboTime = Data.comboSleepTime;

                playerCombat.CanAerialCombo = false;
                playerCombat.IsAerialCombo = false;
            }
        }

        //Reset combo if they have not attacked in a specific amount of time
        if (playerCombat.LastAttackTime < 0 && playerCombat.AttackCounter > 0)
        {
            playerCombat.AttackCounter = 0;

            animator_T.SetFloat("comboRatio", 0);
            animator_B.SetFloat("comboRatio", 0);
        }

        if (playerCombat.LastComboTime < 0)
        {
            playerCombat.CanAerialCombo = true;
        }
    }

    private void UpdateGravityVariables()
    {
        if (!isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
            if (playerCombat.IsAerialCombo)
            {
                SetGravityScale(0);
            }
            //No gravity if the player is sliding
            else if(playerState.IsSliding)
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

    //Check direction that the player should face
    public void CheckDirectionToFace(bool isMovingRight)
    {
        /*Debug.Log("Coyote buffer" + Data.turnCoyoteBuffer);
        Debug.Log(LastOnWallLeftTime);
        Debug.Log(LastOnWallRightTime);*/
        if (isMovingRight != playerState.IsFacingRight)
        {
            if (Data.doTurnOnWallJump)
            {
                if (Data.wallTurnBuffer < LastWallJumpTime)
                    Turn();
            }
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
        return Data.canJump && LastOnGroundTime > 0 && !playerState.IsJumping;
    }

    private bool CanWallJump()
    {
        if (Data.canWallJump)
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
        //Debug.Log("Air: " + airJumpCounter + ". Max: " + maxAirJumps + "!Grounded(): " + !Grounded());
        return Data.canDoubleJump && Data.canDoubleJump && airJumpCounter < Data.maxAirJumps && !Grounded(); //&& isAerialCombo;
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
        if (Data.canDash)
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

    private float OffsetForce(float force)
    {
        return force -= rb.velocity.y;
    }


    //Sleep for delaying movement
    private void Sleep(float duration)
    {
        //Method to help delay time for movement
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private void EndSleep()
    {
        //Method to stop the coroutine from running 
        StopCoroutine(nameof(PerformSleep));
        SetGravityScale(1);
        isSleeping = false;
    }

    private IEnumerator PerformSleep(float duration)
    {
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
            rb.AddForce(new Vector2(direction, 0) * Data.aerialForce, ForceMode2D.Impulse);
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
        if (interactionPrompt.interactable != null)
        {
            interactionPrompt.interactable.PerformInteraction();
        }
    }
}
