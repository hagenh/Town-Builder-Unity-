using DefaultNamespace;
using UnityEngine;
using Tree = DefaultNamespace.Tree;

namespace Player_StateMachine
{
    public class PlayerInteractionState : PlayerState
    {
        private Interactable _target;
        private static readonly int Chop = Animator.StringToHash("Chop");

        public PlayerInteractionState(Interactable interactionTarget)
        {
            _target = interactionTarget;
        }

        public override void EnterState(PlayerController player)
        {
            // PlayInteractionAnimation(_target, player.animator);
        }

        public override void UpdateState(PlayerController player)
        {
            if (_target.isDepleted)
            {
                Debug.Log($"Interaction complete! isDepleted: {_target.isDepleted}");
                player.stateMachine.SwitchState(new PlayerIdleState());
                return;
            }

            HandleResourceGathering(_target, player);
        }

        public override void ExitState(PlayerController player)
        {
            // player.StopInteraction();
        }

        public void PlayInteractionAnimation(Interactable target, Animator animator) => animator.SetTrigger(target switch
        {
            Tree => "Chop",
            Rock => "Mine",
            _ => "Interact"
        });
        
        public void OnGUI()
        {
            GUI.Label(new Rect(_target.transform.position.x, _target.transform.position.y - 100, 100, 20), "Press E to interact");
        }
        
        public void HandleResourceGathering(Interactable target, PlayerController player)
        {
            target.health -= Time.deltaTime; // Example damage calculation
            //player.animator.SetTrigger(Chop);
            
            Debug.Log("Interactable Health: " + target.health);
        
            if (target.health <= 0)
            {
                Debug.Log("Interactable dead!: " + target.health);
                
                target.Interact(player);
                player.stateMachine.SwitchState(new PlayerIdleState());
            }
        }
    }
}