using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackSoundPlay : StateMachineBehaviour
{
    [SerializeField] public EventReference swingFX;
    [SerializeField] public EventReference spinFX;
    [SerializeField] public EventReference slamFX;
    [SerializeField] protected EventInstance attackInstance;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        string currentAnimationState = PlayerAnimationManager.Instance.CurrentState;
        Debug.Log(currentAnimationState);

        switch(currentAnimationState)
        {
            case "Attack1":
            case "Attack2":
                attackInstance = RuntimeManager.CreateInstance(swingFX);
                break;
            case "Attack3":
                attackInstance = RuntimeManager.CreateInstance(spinFX);
                break;
            case "Attack4":
                attackInstance = RuntimeManager.CreateInstance(slamFX);
                break;
            default:
                attackInstance = RuntimeManager.CreateInstance(swingFX);
                break;

        }

        attackInstance.start();
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Just Updated");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
