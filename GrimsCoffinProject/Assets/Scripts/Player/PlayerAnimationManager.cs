using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimationStates
{
    public static readonly string Idle = "Idle";
    public static readonly string Run = "Run";
    public static readonly string JumpUp = "JumpUp";
    public static readonly string JumpDown = "JumpDown";
    public static readonly string Dash = "Dash";
    public static readonly string GroundCharge = "GroundCharge";
    public static readonly string GroundDown = "GroundDown";
    public static readonly string WallSlide = "WallSlide";
    public static readonly string Attack1 = "Attack1";
    public static readonly string Attack2 = "Attack2";
    public static readonly string Attack3 = "Attack3";
    public static readonly string AttackUp = "AttackUp";
    public static readonly string Attack4 = "Attack4";
    public static readonly string Death = "Death";
    public static readonly string AttackDownReady = "AttackDownReady";
    public static readonly string AttackDownExecute = "AttackDownExecute";

    public static string GetComboAnimation(int index)
    {
        switch (index)
        {
            case 1:
                return Attack1;
            case 2:
                return Attack2;
            case 3:
                return Attack3;
            case 4:
                return AttackUp;
            default:
                return Attack1;
        }
    }
}

public class PlayerAnimationManager : MonoBehaviour
{
    public static PlayerAnimationManager Instance;

    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Animator scytheAnimator;

    private string currentState;

    public string CurrentState { get { return currentState; } }

    public float GetCurrentAnimationLength()
    {
        if (currentState == null) return 0;

        return playerAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeAnimationState(string newState, bool waitForEnd = false)
    {
        //Don't change animation if game is paused
        if (UIManager.Instance.pauseScript.isPaused)
            return;

        //Also don't change animation if map screen is active
        if (UIManager.Instance.fullMapUI != null)
        {
            if (UIManager.Instance.fullMapUI.activeInHierarchy)
                return;
        }

        // Stop animation from interrupting itself
        if (currentState == newState) return;

        playerAnimator.Play(newState);
        scytheAnimator.Play(newState);

        currentState = newState;
    }

    public void ChangeAnimationSpeed(float speed)
    {
        playerAnimator.speed = speed;
        scytheAnimator.speed = speed;
    }

    public void ChangeSpriteLayer(int layer)
    {
        playerAnimator.gameObject.layer = layer;
        scytheAnimator.gameObject.layer = layer;
    }
}
