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
        // playerController.WalkModifier = PlayerControllerForces.Instance.Data.gDownWalkModifier;
        playerController.SleepWalk();
        playerCombat.AttackDurationTime = 0f;

        PlayerAnimationManager.Instance.ChangeAnimationState(PlayerAnimationStates.AttackDownReady);
    }

    public override void OnUpdate(CStateMachine _stateMachine)
    {
        if (_stateMachine.RegisteredAttack)
        {
            stateMachine.SetNextState(new GroundDownRelease());
            _stateMachine.RegisteredAttack = false;
            playerController.EndSleepWalk();
        }
        else if (!playerCombat.isHoldingDownOnGround)
        {
            stateMachine.SetNextStateToMain();
            playerController.EndSleepWalk();
        }
        //Highlight yellow after certain time to note that the attack is charged
        //if(playerCombat.AttackDurationTime < 0)
        //{
        //    playerCombat.scytheSprite.GetComponent<SpriteRenderer>().color = Color.yellow;
        //}

        //if (!playerCombat.IsHoldingAttacking)
        //{
        //    playerController.EndSleepWalk();

        //    //Released after being charged up
        //    if (playerCombat.AttackDurationTime < 0)
        //    {
        //        playerCombat.scytheSprite.GetComponent<SpriteRenderer>().color = Color.white;

        //        stateMachine.SetNextState(new GroundDownRelease());
        //    }
        //    //Let go of attack before charging up
        //    else 
        //    {
        //        Debug.Log("Ended holding");
        //        playerController.WalkModifier = 1;
        //        playerCombat.AttackDurationTime = 0;
        //        stateMachine.SetNextStateToMain();
        //    }
        //}
    }
}
