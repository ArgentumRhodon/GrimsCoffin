using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }
    public float LastJumpTime { get; private set; }

    //Jump
    private bool isJumpCancel;
    private bool isJumpFalling;
    private int airJumpCounter = 0;
    float jumpClamp;

    //Wall Jump
    private float wallJumpStartTime;
    private int lastWallJumpDir;

    //Dash
    private int dashesLeft;
    private bool dashRefilling;
    private Vector2 lastDashDir;
    private bool isDashAttacking;

    //Attack
    private bool canAerialCombo;
    private bool isAerialCombo;
    private int attackCounter;

    //Input parameters
    private Vector2 moveInput;

    // Animation Stuff
    [SerializeField] private Animator animator_T; // Top
    [SerializeField] private Animator animator_B; // Bottom

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }

    public float LastComboTime { get; private set; }
    public float LastAttackTime { get; private set; }

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
    [SerializeField] private int maxAirJumps;
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public float maxSP;
    [SerializeField] public float currentSP;
    [SerializeField] public float invincibilityTimer;
    [SerializeField] public bool hasInvincibility;

    [Header("Player UI")]
    [SerializeField] public InteractionPrompt interactionPrompt;
    public bool canInteract = false;

    //Singleton so the controller can be referenced across scripts
    public static PlayerControllerForces Instance;

    //Reference to Player Input/Controls
    private PlayerControls playerControls;

    //Time Variables
    private float localDeltaTime;
    private Time localTimeTest;
    private bool isSleeping;


    private void Awake()
    {
        //Set rigidbody
        rb = GetComponent<Rigidbody2D>();

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
        canAerialCombo = true;
        isSleeping = false;

        respawnPoint = this.transform.position; 
    }

    private void Update()
    {
        //Update all timers -----------------------------------
        /*        if (!isSleeping)
                {
                    localDeltaTime = Time.deltaTime;
                    //localTimeTest = Time.
                }

                //Collision Checks
                LastOnGroundTime -= localDeltaTime;
                LastOnWallTime -= localDeltaTime;
                LastOnWallRightTime -= localDeltaTime;
                LastOnWallLeftTime -= localDeltaTime;

                //Movement
                LastPressedJumpTime -= localDeltaTime;
                LastPressedDashTime -= localDeltaTime;
                if (playerState.IsJumping)
                {
                    LastJumpTime += localDeltaTime;
                    //Debug.Log(LastJumpTime);
                }

                //Combat
                LastComboTime -= localDeltaTime;
                LastAttackTime -= localDeltaTime;*/
        if (!isSleeping)
        {
            //Collision Checks
            LastOnGroundTime -= Time.deltaTime;
            LastOnWallTime -= Time.deltaTime;
            LastOnWallRightTime -= Time.deltaTime;
            LastOnWallLeftTime -= Time.deltaTime;

            //Movement
            LastPressedJumpTime -= Time.deltaTime;
            LastPressedDashTime -= Time.deltaTime;
            if (playerState.IsJumping)
            {
                LastJumpTime += Time.deltaTime;
                //Debug.Log(LastJumpTime);
            }

            //Combat
            LastComboTime -= Time.deltaTime;
            LastAttackTime -= Time.deltaTime;

            //Movement/Walking input
            moveInput = playerControls.Player.Move.ReadValue<Vector2>();

            //Check direction of player
            if (moveInput.x != 0)
                CheckDirectionToFace(moveInput.x > 0);

            //Check if player hit ground or walls
            CollisionChecks();

            //Update movement variables
            UpdateJumpVariables();
            UpdateDashVariables();

            //Slide Check
            UpdateSlideVariables();

            //Attack Check
            UpdateAttackVariables();

            //Gravity check
            UpdateGravityVariables();
        }
        else if(isAerialCombo)
        {
            //Combat
            LastComboTime -= Time.deltaTime;
            LastAttackTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(isSleeping)
            if (moveInput.x != 0)
                if(LastAttackTime < 0)
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
        if (isSleeping)
            EndSleep();

        //Values to check if the key is down or up - will determine if the jump should be canceled or not
        if (value.isPressed)
        {
            jumpClamp = LastJumpTime;
            LastPressedJumpTime = Data.jumpInputBufferTime;
            LastJumpTime = 0;
        }
        else if (!value.isPressed)
        {
            if (CanJumpCancel() || CanWallJumpCancel())
                isJumpCancel = true;
        }

        if (!playerState.IsDashing && value.isPressed)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                playerState.IsJumping = true;
                playerState.IsWallJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;
                Jump();
            }
            //Wall Jump
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                playerState.IsWallJumping = true;
                playerState.IsJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;

                //Set timer and direction of jump
                wallJumpStartTime = Time.time;
                lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                //Stops player from double jumping off wall
                airJumpCounter = maxAirJumps; 

                WallJump(lastWallJumpDir);
            }
            //Double jump
            else if (CanDoubleJump())
            {
                playerState.IsJumping = true;
                playerState.IsWallJumping = false;
                isJumpCancel = false;
                isJumpFalling = false;
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
        Debug.Log("Dash");
    }

    private void OnPause()
    {
        UIManager.Instance.Pause();
    }

    private void OnAttack()
    {
        //Combo attack counter
        attackCounter++;

        animator_T.SetFloat("comboRatio", attackCounter / 4f);
        animator_T.SetFloat("comboRatio", attackCounter / 4f);
        animator_T.SetTrigger("Attack");
        animator_B.SetTrigger("Attack");

        //Aerial attack
        if (!Grounded())
        {
            if (canAerialCombo && LastComboTime < 0)
            {
                Debug.Log(attackCounter > 1);
                isAerialCombo = true;
                Sleep(Data.comboSleepTime);
                LastAttackTime = Data.attackBufferTime;
            }
        }

        //Reset combo attack counter after a time delay
        if (attackCounter >= 4)
        {
            LastComboTime = Data.comboSleepTime;
            attackCounter = 0;
        }
    }

    public void TakeDamage(float damageTaken)
    {
        currentHP -= damageTaken;
        invincibilityTimer = 2.0f;
        hasInvincibility = true;

        UIManager.Instance.DamageVignette();

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
            if (playerState.IsFacingRight)
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

        //Convert movement to a vector and apply it
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    //Used for player direction
    private void Turn()
    {
        if (Time.timeScale == 0)
            return;
        //Scale of sprite
        //playerState.IsFacingRight = !playerState.IsFacingRight;
        //this.gameObject.GetComponent<SpriteRenderer>().flipX = !playerState.IsFacingRight;

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

        //Increase the force applied if we are falling
        //Debug.Log("Rb velocity " + rb.velocity.y);

        float force = Data.jumpForce;
        if (rb.velocity.y < 0.01f)
            force -= rb.velocity.y;
        else if (rb.velocity.y > .1f)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        //Clamp to calculate whether the player is already jumping or not to make sure the forces don't stack too high
/*        if (CanDoubleJump()) 
        {
            //Debug.Log("Jump clamping: " + jumpClamp);
            //force = Mathf.Clamp(force * jumpClamp, force * 0.5f, force);
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            //rb.AddForce(Vector2.up * force * Data.doubleJumpMultiplier, ForceMode2D.Impulse);
            jumpClamp = 0;
        }
        else
        {
            Debug.Log(rb.velocity.y);
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }*/
    }

    //Wall jump
    private void WallJump(int dir)
    {
        //Ensures we can't call wall jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        //Perform Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling
            force.y -= rb.velocity.y;

        //Use impulse to apply force instantly ignoring mass
        rb.AddForce(force, ForceMode2D.Impulse);

    }

    //Slide
    private void Slide()
    {
        //Remove the remaining upwards Impulse to prevent upwards sliding
        if (rb.velocity.y > 0)
        {
            rb.AddForce(-rb.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        //Slide
        float speedDif = Data.slideSpeed - rb.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //Clamp the movement here to prevent any over corrections
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }

    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        //Dash check
        Debug.Log("Dash");
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        dashesLeft--;
        isDashAttacking = true;

        SetGravityScale(0);

        Sleep(0.1f);

        int direction = XInputDirection();
        //If player is not moving / doesn't have direction, dash in the most recent input
        if(direction == 0)
        {
            if (playerState.IsFacingRight)
                direction = 1;
            else
                direction = -1;
        }

        //We keep the player's velocity at dash speed
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            rb.velocity = new Vector2(direction * Data.dashSpeed,0);
            //Pauses the loop until the next frame
            yield return null;
        }

        startTime = Time.time;

        isDashAttacking = false;

        //Reset movement back to close to the walking speed
        SetGravityScale(Data.gravityScale);
        rb.velocity = new Vector2(Data.dashEndSpeed.x * direction,0);

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
        else if (OnWall())
        {
            //airJumpCounter = 0;
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
        if (CanSlide() && ((LastOnWallLeftTime > 0 && moveInput.x < 0) || (LastOnWallRightTime > 0 && moveInput.x > 0)))
            playerState.IsSliding = true;
        else
            playerState.IsSliding = false;
    }

    private void UpdateAttackVariables()
    {
        if (isAerialCombo)
        {
           if(LastAttackTime < 0)
           {
                LastComboTime = Data.comboSleepTime;

                canAerialCombo = false;
                isAerialCombo = false;
            }
        }

        //Reset combo if they have not attacked in a specific amount of time
        if (LastAttackTime < 0 && attackCounter > 0)
        {
            attackCounter = 0;

            animator_T.SetFloat("comboRatio", 0);
            animator_B.SetFloat("comboRatio", 0);
        }

        if (LastComboTime < 0)
        {
            canAerialCombo = true;
        }
    }

    private void UpdateGravityVariables()
    {
        if (!isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
/*            if (isAerialCombo)
            {
                SetGravityScale(0);
            }
            else*/
            if (rb.velocity.y < 0 && moveInput.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (isJumpCancel)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCancelGravityMult);
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
        if (isMovingRight != playerState.IsFacingRight)
            Turn();
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
        return LastOnGroundTime > 0 && !playerState.IsJumping;
    }

    private bool CanWallJump()
    {
        /*Debug.Log(//LastPressedJumpTime > 0); //&&
                  LastOnWallTime > 0);//  && LastOnGroundTime <= 0 && (!playerState.IsWallJumping ||
             //(LastOnWallRightTime > 0 && lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && lastWallJumpDir == -1)));*/

        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!playerState.IsWallJumping ||
             (LastOnWallRightTime > 0 && lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool CanDoubleJump()
    {
        //Debug.Log("Air: " + airJumpCounter + ". Max: " + maxAirJumps + "!Grounded(): " + !Grounded());
        return airJumpCounter < maxAirJumps && !Grounded(); //&& isAerialCombo;
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
        if (!playerState.IsDashing && dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !dashRefilling) //(LastOnGroundTime > 0 || OnWall())
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return dashesLeft > 0;
    }

    //Check to see if the player is on the wall and is sliding
    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !playerState.IsJumping && !playerState.IsWallJumping && !playerState.IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }

    //Set gravity 
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
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
        //Debug.Log("Sleeping");

        //Time.timeScale = 0;
        //localDeltaTime = 0;
        SetGravityScale(0);
        isSleeping = true;

        //Combat force calculations
        if (isAerialCombo)
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
        if (isAerialCombo)
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