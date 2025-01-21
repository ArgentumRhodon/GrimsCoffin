using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDownCharge : MeleeBaseState
{
    PlayerControllerForces playerController;

    public override void OnEnter(CStateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        playerController = playerCombat.GetComponent<PlayerControllerForces>();
        playerController.WalkModifier = playerCombat.Data.gDownWalkModifier;
        playerController.SleepWalk();

        animator.SetTrigger("GroundCharge");
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
            playerController.EndSleep();

            //Released after being charged up
            if (playerCombat.AttackDurationTime < 0)
            {
                playerCombat.scytheSprite.GetComponent<SpriteRenderer>().color = Color.white;

                stateMachine.SetNextState(new GroundDownRelease());
            }
            //Let go of attack before charging up
            else 
            {
                playerController.WalkModifier = 1;
                stateMachine.SetNextStateToMain();
            }
        }
    }
}
