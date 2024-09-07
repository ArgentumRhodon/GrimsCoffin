using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //Private variables
    private Rigidbody2D rb;
    private float xAxis;
    //private float yAxis;
    PlayerStateList playerState;
    private float gravity;

    //Editable Settings
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1f;

    [Header("Jump Movement Settings")]
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float jumpCancelForce = 0.25f;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

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
    [SerializeField] private bool dashed;

    //Singleton so the controller can be referenced across scripts
    public static PlayerController Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        //Update current states
        GetInputs();
        UpdateJumpVariables();
        //Flip();

        //Controls, makes sure it doesn't take input when in certain movements
        if (playerState.dashing)
            return;
       
        Move();
        Jump();
        StartDash();
    }

    
    private void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        //yAxis = Input.GetAxisRaw("W");
    }

    //Basic back and forth movement
    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
    }

    //Check what direction the player is moving in
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
    }

    //Jump
    private void Jump()
    {
        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - jumpCancelForce);

            //Check to make sure there is not more downwards velocity, gravity should be the main force controlling this
            if (rb.velocity.y <= 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            playerState.jumping = false;
        }

        if (!playerState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                playerState.jumping = true;
            }
            else if(!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                playerState.jumping = true;
                airJumpCounter++;

                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }
    }

    //Jump Helper Methods
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundLayer))
            return true;
        else
            return false;
    }

    private void UpdateJumpVariables()
    {
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

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    //Method to run Dash IEnum
    private void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded()) 
        { 
            dashed = false;
        }
    }

    //Function that controls the dash movement
    IEnumerator Dash()
    {
        canDash = false;
        playerState.dashing = true;
        rb.gravityScale = 0;
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector2(0, transform.localScale.y * upwardsDashSpeed);
            yield return new WaitForSeconds(upwardsDashTime);

            while(rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - upDashCancelForce);
            }
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        else
        {
            if (xAxis > 0)
            {
                rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            }
            else if (xAxis < 0)
            {
                rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
            }

            //rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);

            yield return new WaitForSeconds(dashTime);
        }

 
        rb.gravityScale = gravity;
        playerState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
