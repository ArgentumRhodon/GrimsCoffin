using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerAnimationStates
{
    public static readonly string Idle = "Idle";
    public static readonly string Run = "Run";
    public static readonly string Jump = "Jump";
    public static readonly string Dash = "Dash";
    public static readonly string GroundCharge = "GroundCharge";
    public static readonly string GroundDown = "GroundDown";
    public static readonly string Attack1 = "Attack1";
    public static readonly string Attack2 = "Attack2";
    public static readonly string Attack3 = "Attack3";
    public static readonly string Attack4 = "Attack4";

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
                return Attack4;
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

    public void ChangeAnimationState(string newState)
    {
        // Stop animation from interrupting itself
        if (currentState == newState) return;

        playerAnimator.Play(newState);
        scytheAnimator.Play(newState);

        currentState = newState;
    }
}
