using Player_StateMachine;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    
    public override void EnterState(PlayerController player)
    {
        ResetAnimations(player);
    }

    public override void UpdateState(PlayerController player)
    {
        if (player.movement.sqrMagnitude > 0)
        {
            player.stateMachine.SwitchState(new PlayerMovementState());
        }
    }

    public override void ExitState(PlayerController player) { }
    
    public void ResetAnimations(PlayerController player) => player.animator.SetFloat(Speed, 0);

}
