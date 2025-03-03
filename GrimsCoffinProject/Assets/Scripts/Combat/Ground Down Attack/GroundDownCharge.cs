using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDownCharge : MeleeBaseState
{
    PlayerControllerForces playerController;

    public GroundDownCharge() : base()
    {
        attackIndex = -1; // Not part of combo
    }

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        playerController = playerCombat.GetComponent<PlayerControllerForces>();
        playerController.WalkModifier = playerCombat.Data.gDownWalkModifier;
        playerController.SleepWalk();
        playerCombat.AttackDurationTime = playerCombat.Data.gdHoldDuration;


        PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.GroundCharge);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        //Highlight yellow after certain time to note that the attack is charged
        if(playerCombat.AttackDurationTime < 0)
        {
            playerCombat.scytheSprite.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        if (!playerCombat.IsHoldingAttacking)
        {
            playerController.EndSleepWalk();

            //Released after being charged up
            if (playerCombat.AttackDurationTime < 0)
            {
                playerCombat.scytheSprite.GetComponent<SpriteRenderer>().color = Color.white;

                stateMachine.SetNextState(new GroundDownRelease());
            }
            //Let go of attack before charging up
            else 
            {
                Debug.Log("Ended holding");
                playerController.WalkModifier = 1;
                playerCombat.AttackDurationTime = 0;
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
