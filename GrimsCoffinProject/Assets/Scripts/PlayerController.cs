using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    //Private variables
    private Rigidbody2D rb;
    //private float xAxis;
    //private float yAxis;
    PlayerStateList playerState;
    private float gravity;

    private Vector2 input;

    //Editable Settings
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1f;

    [Header("Jump Movement Settings")]
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float jumpCancelForce = 0.25f;
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    //[SerializeField] private bool canJump;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dash Movement Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float upwardsDashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float upwardsDashTime;
    [SerializeField] private float upDashCancelForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private bool canDash;
    private float currentDashCooldown;

    [Header("Player Stats")]
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public float maxSP;
    [SerializeField] public float currentSP;

    //Singleton so the controller can be referenced across scripts
    public static PlayerController Instance;

    [Header("Input Actions")]
    //[SerializeField] private InputAction moveAction;
    private PlayerControls playerControls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();

        gravity = rb.gravityScale;
        canDash = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        //Update movement variables
        UpdateJumpVariables();
        UpdateDashVariables();
        //Flip();

        Move();
    }

    //Walking
    private void Move()
    {
        if (playerState.IsDashing)
            return;

        input = playerControls.Player.Move.ReadValue<Vector2>();
        rb.velocity = new Vector2(walkSpeed * XInputDirection(), rb.velocity.y);
    }
/*
    //Walking
    private void OnMove(InputValue value)
    {
        if (playerState.dashing)
            return;

        //Debug.Log("Player is moving");

        input = value.Get<Vector2>();

        if (value.isPressed)
            Debug.Log("is pressed");
    }*/

    //Jump
    private void OnJump(InputValue value)
    {
        //Return if player is mid-dash
        if (playerState.IsDashing)
            return;

        jumpBufferCounter = jumpBufferFrames;

        //Jump cancel if released
        if (!value.isPressed && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - jumpCancelForce);

            //Check to make sure there is not more downwards velocity, gravity should be the main force controlling this
            if (rb.velocity.y <= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            playerState.IsJumping = false;
        }

        //If the player is not jumping already, activate jump
        if (value.isPressed && !playerState.IsJumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                playerState.IsJumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps)
            {
                playerState.IsJumping = true;
                airJumpCounter++;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }
    }


    //Method to run Dash IEnum
    private void OnDash()
    {
        //Checks to see if the player can dash (cooldown is reset) and makes sure player is not currently dashing
        if (canDash && !playerState.IsDashing)
        {
            //Update States
            playerState.IsDashing = true;
            canDash = false;

            //Cooldown Timer
            currentDashCooldown = dashCooldown;

            //Dash
            StartCoroutine(Dash());
        }
    }

    //Function that controls the dash movement
    IEnumerator Dash()
    {
        //Update physics of player
        rb.gravityScale = 0;

        //Get direction of input
        int direction = XInputDirection();
        //Debug.Log(direction);

        rb.velocity = new Vector2(transform.localScale.x * dashSpeed * direction, 0);

        //Dash movement has been completed
        yield return new WaitForSeconds(dashTime);
        playerState.IsDashing = false;
        rb.gravityScale = gravity;

        //Reset velocity, may need to be changed with implementation of phantom shift
        rb.velocity = new Vector2(walkSpeed * input.x, rb.velocity.y);
    }


    /*    //Check what direction the player is moving in
        private void Flip()
        {
            if (xAxis < 0)
            {
                transform.localScale = new Vector2(-1, transform.localScale.y);
            }
            else if (xAxis > 0)
            {
                transform.localScale = new Vector2(1, transform.localScale.y);
            }
        }*/

    //Helper Methods ----------------------------------------------------------------------------------------------

    //Checks to see if the user is grounded
    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer);
    }

    private bool OffWall()
    {
        return Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer);
    }

    private int XInputDirection()
    {
        if (input.x == 0)
            return 0;
        else if (input.x < 0)
            return -1;
        else
            return 1;
    }

    //Methods to update movement variables ------------------------------------------------------------------------

    //Jump
    private void UpdateJumpVariables()
    {
        //Checks grounded state to determine if player can jump again
        if (Grounded())
        {
            playerState.IsJumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //Check for jump delay
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
    }

    //Dash
    private void UpdateDashVariables()
    {
        //Checks grounded state to determine if player can jump again
        if (Grounded() && currentDashCooldown < 0)
        {
            canDash = true;
        }
    //Method to run Dash IEnum
    //private void OnDash()
    //{
    //    if (canDash && !dashed) 
    //    {
    //        dashed = true;
    //        StartCoroutine(Dash());
    //    }
    //}

        //Update dash cooldown
        if (currentDashCooldown > 0)
        {
            currentDashCooldown = currentDashCooldown - Time.deltaTime;
        }
    }
    private void OnPause()
    {
        UIManager.Instance.Pause();
    }
}
