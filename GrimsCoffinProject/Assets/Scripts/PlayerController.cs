using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

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

    [Header("Player Stats")]
    [SerializeField] public float maxHP;
    [SerializeField] public float currentHP;
    [SerializeField] public float maxSP;
    [SerializeField] public float currentSP;

    //Singleton so the controller can be referenced across scripts
    public static PlayerController Instance;

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
        //Update movement variables
        UpdateJumpVariables();
        UpdateDashVariables();
        //Flip();
    }

    //Walking
    private void OnMove(InputValue value)
    {
        if (playerState.dashing)
            return;

        Debug.Log("Player is moving");

        input = value.Get<Vector2>();
        rb.velocity = new Vector2(walkSpeed * input.x, rb.velocity.y);
    }

    //Jump
    private void OnJump(InputValue value)
    {
        Debug.Log("Player is jumping");

        //Return if player is mid-dash
        if (playerState.dashing)
            return;

        jumpBufferCounter = jumpBufferFrames;
        //Debug.Log(value.isPressed);

        //Jump cancel if released
        if (!value.isPressed && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - jumpCancelForce);

            //Check to make sure there is not more downwards velocity, gravity should be the main force controlling this
            if (rb.velocity.y <= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            playerState.jumping = false;
        }

        //If the player is not jumping already, activate jump
        if (value.isPressed && !playerState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                playerState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps)
            {
                playerState.jumping = true;
                airJumpCounter++;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }
    }


    //Method to run Dash IEnum
    private void OnDash()
    {
        //Checks to see if the player can dash (cooldown is reset) and makes sure player is not currently dashing
        if (canDash && !playerState.dashing)
        {
            playerState.dashing = true;
            canDash = false;
            StartCoroutine(Dash());
        }
    }

    //Function that controls the dash movement
    IEnumerator Dash()
    {
        //Update physics of player
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed * input.x, 0);

        //Dash movement has been completed
        yield return new WaitForSeconds(dashTime);
        playerState.dashing = false;
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

    //Methods to update movement variables ------------------------------------------------------------------------

    //Jump
    private void UpdateJumpVariables()
    {
        //Checks grounded state to determine if player can jump again
        if (Grounded())
        {
            playerState.jumping = false;
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
        if (Grounded() && dashCooldown < 0)
        {
            canDash = true;
        }

        //Update dash cooldown
        if (dashCooldown > 0)
        {
            dashCooldown = dashCooldown - Time.deltaTime;
        }
    }
}
