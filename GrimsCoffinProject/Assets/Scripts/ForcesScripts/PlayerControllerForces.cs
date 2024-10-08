using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

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

    //Jump
    private bool isJumpCancel;
    private bool isJumpFalling;

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
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public float maxSP;
    [SerializeField] public float currentSP;


    //Singleton so the controller can be referenced across scripts
    public static PlayerControllerForces Instance;

    //Reference to Player Input/Controls
    private PlayerControls playerControls;


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
    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    private void Start()
    {
        //Get player state and set default values
        playerState = GetComponent<PlayerStateList>();
        SetGravityScale(Data.gravityScale);
        playerState.IsFacingRight = true;
    }

    private void Update()
    {
        //Update all timers
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;

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

        //Gravity check
        UpdateGravityVariables();
    }

    private void FixedUpdate()
    {
        //Handle player walking, make sure the player doesn't walk while dashing
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

    //Input Methods ----------------------------------------------------------------------------------------------
    //Jump Input
    private void OnJump(InputValue value)
    {
        //Values to check if the key is down or up - will determine if the jump should be canceled or not
        if (value.isPressed)
        {
            LastPressedJumpTime = Data.jumpInputBufferTime;
        }
        else if (!value.isPressed)
        {
            if (CanJumpCancel() || CanWallJumpCancel())
                isJumpCancel = true;
        }
    }

    //Dash Input
    private void OnDash()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }

    //Walking
    private void Walk(float lerpAmount)
    {
        //Calculate the direction and our desired velocity
        float targetSpeed = moveInput.x * Data.walkMaxSpeed;
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
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        playerState.IsFacingRight = !playerState.IsFacingRight;
    }

    //Jump
    private void Jump()
    {
        //Ensures we can't call jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        //Increase the force applied if we are falling
        float force = Data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

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

    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        //Dash check
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        dashesLeft--;
        isDashAttacking = true;

        SetGravityScale(0);

        //We keep the player's velocity at dash speed
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            rb.velocity = dir.normalized * Data.dashSpeed;
            //Pauses the loop until the next frame
            yield return null;
        }

        startTime = Time.time;

        isDashAttacking = false;

        //Reset movement back to close to the walking speed
        SetGravityScale(Data.gravityScale);
        rb.velocity = Data.dashEndSpeed * dir.normalized;

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

        if (!playerState.IsDashing)
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

                wallJumpStartTime = Time.time;
                lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                WallJump(lastWallJumpDir);
            }
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

    private void UpdateGravityVariables()
    {
        if (!isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
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
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight)
                    || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight)) && !playerState.IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            //Left Wall Check
            if (((Physics2D.OverlapBox(frontWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !playerState.IsFacingRight)
                || (Physics2D.OverlapBox(backWallCheckPoint.position, wallCheckSize, 0, groundLayer) && playerState.IsFacingRight)) && !playerState.IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != playerState.IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !playerState.IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!playerState.IsWallJumping ||
             (LastOnWallRightTime > 0 && lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && lastWallJumpDir == -1));
    }

    private bool CanJumpCancel()
    {
        return playerState.IsJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCancel()
    {
        return playerState.IsWallJumping && rb.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!playerState.IsDashing && dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return dashesLeft > 0;
    }

    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method to help delay time for movement
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(frontWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(backWallCheckPoint.position, wallCheckSize);
    }
}
