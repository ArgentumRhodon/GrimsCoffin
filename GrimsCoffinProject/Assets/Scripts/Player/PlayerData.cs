using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    public float deadzone;
    public float cameraOffset;
    public float cameraWalkOffset;
    public float cameraDashOffset;

    [Header("Gravity")]
    [HideInInspector] public float gravityStrength; //Downwards force needed for the desired jumpHeight and jumpTimeToApex
    [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity
                                                 
    [Space(5)]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling
    public float maxFallSpeed; //Maximum fall speed of the player when falling
    [Space(5)]
    public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed
    public float maxFastFallSpeed; //Maximum fall speed of the player when performing a faster fall

    [Space(20)]

    [Header("Walk")]
    public float walkMaxSpeed; //Target speed we want the player to reach.
    public float walkAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
    [HideInInspector] public float walkAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float walkDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
    [HideInInspector] public float walkDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
    [Space(5)]
    [Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0f, 1)] public float deccelInAir;
    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("Jump")]
    public bool canJump;
    public float jumpHeight; //Height of the player's jump
    public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
    [HideInInspector] public float jumpForce; //The actual force applied to the player when they jump.

    [Space(10)]

    [Header("All Jumps")]
    public float jumpCancelGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
    [Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    [Space(0.5f)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Space(10)]

    [Header("Wall Jump")]
    public bool canWallJump;
    public bool canWallJumpCancel;
    public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
    [Space(10)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
    public bool mustHoldWallToJump;
    [Range(0.01f, 0.5f)] public float wallCoyoteTime; //Grace period after falling off a platform, where you can still jump
    [Space(10)]
    public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction
    [Range(0f, 0.5f)] public float wallTurnBuffer;

    [Space(10)]

    [Header("Extra Jumps")]
    public bool canDoubleJump;
    public bool preserveDoubleJump;
    public bool resetJumpOnWall;
    public float extraJumpMultiplier; //Multiplier for impulse added on double jump
    public int maxAirJumps; //Amount of total jumps 

    [Space(20)]

    [Header("Slide")]
    public bool canSlide;
    public float slideSpeed;
    public float slideAccel;

    [Space(20)]

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    [Space(20)]

    [Header("Dash")]
    public bool canDash;
    public int dashAmount;
    public float dashSpeed;
    public float dashSleepTime; //Duration for which the game freezes when we press dash but before we read directional input and apply a force
    [Space(5)]
    public float dashAttackTime;
    [Space(5)]
    public float dashEndTime; //Time after you finish the inital drag phase, smoothing the transition back to idle
    public Vector2 dashEndSpeed; //Slows down player, makes dash feel more responsive 
    [Range(0f, 1f)] public float dashEndRunLerp; //Slows the affect of player movement while dashing
    [Space(5)]
    public float dashRefillTime;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float dashInputBufferTime;

    [Space(20)]

    [Header("General Attacks")]
    public bool canAttack;

    [Space(10)]

    [Header("Main Combo Attack")]
    public bool canTurnDuringCombo;
    public float comboSleepTime; //Sleep time after combo
    public float attackBufferTime; //Attack buffer to track if the player should stay in the combo or not
    public float comboAerialTime; //Time player is in the air
    public float comboTotal; //Length of the combo
    public float comboAerialPForce;

    [Space(15)]

    public float combo1Damage;
    public float combo2Damage;
    public float combo3Damage;
    public float combo4Damage;

    [Space(15)]

    [Header("Ground Up Attack")]
    public float groundUpwardPForce;
    public float groundUpwardEForce;

    [Space(15)]

    public float groundUpDamage;

    [Space(15)]

    [Header("Ground Down Attack")]
    public float groundDownwardPForce;
    public float groundDownwardEForce;
    public float gdHoldDuration;

    [Space(15)]

    public float groundDownDamage;

    [Space(15)]

    [Header("Aerial Up Attack")]
    public float hookPlayerForce;

    [Space(15)]

    public float aerialUpDamage;

    [Space(15)]

    [Header("Aerial Down Attack")]
    public float aerialDownwardPForce;
    public float aerialDownwardEForce;

    [Space(15)]

    public float aerialDownDamage;
    public float aerialImpactDamage;

    //Unity Callback, called when the inspector updates
    private void OnValidate()
    {
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        gravityScale = gravityStrength / Physics2D.gravity.y;

        //Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
        walkAccelAmount = (50 * walkAcceleration) / walkMaxSpeed;
        walkDeccelAmount = (50 * walkDecceleration) / walkMaxSpeed;

        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        //Variable ranges
        walkAcceleration = Mathf.Clamp(walkAcceleration, 0.01f, walkMaxSpeed);
        walkDecceleration = Mathf.Clamp(walkDecceleration, 0.01f, walkMaxSpeed);
    }
}
