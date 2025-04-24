using UnityEngine;

namespace Player_StateMachine
{
    public class PlayerMovementState : PlayerState
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        public override void EnterState(PlayerController player)
        {
            // PlayMovementAnimation(player);
        }

        public override void UpdateState(PlayerController player)
        {
            HandleMovement(player);
        
            if (player.movement.sqrMagnitude == 0)
            {
                player.stateMachine.SwitchState(new PlayerIdleState());
            }
        }

        public override void ExitState(PlayerController player) { }
        
        public void PlayMovementAnimation(PlayerController player) => player.animator.SetFloat(Speed, 1);
        
        
        public void HandleMovement(PlayerController player)
        {
            player.movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            player.rb.linearVelocity = player.movement * player.moveSpeed;

            // Animation handling
            /*player.animator.SetFloat(Speed, player.movement.magnitude);
            if (player.movement.x != 0)
            {
                player.transform.localScale = new Vector3(Mathf.Sign(player.movement.x), 1, 1);
            }*/
        }

    }
}