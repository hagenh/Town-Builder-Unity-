using DefaultNamespace;
using UnityEngine;

namespace Player_StateMachine
{
    public class PlayerCombatState : PlayerState
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        private Vector2 _startingPosition = new Vector2(-0.1f, 0-5f);

        private GameObject _currentWeapon;
        private Vector3 _pivot;
        private float _rotated = 0f;
        private const float TotalAngle = 160f;
        private const float Duration = 0.5f;

        public override void EnterState(PlayerController player)
        {
            Debug.Log("Prefab to spawn: " + GameManager.Instance.CurrentItem.prefab);
            
            _startingPosition = player.transform.position.ToVector2() + new Vector2(-0.1f, 0.5f);

            // DrawWeapon(player);
            _currentWeapon = GameObject.Instantiate(GameManager.Instance.CurrentItem.prefab, _startingPosition, Quaternion.identity, player.transform);
            _pivot = player.transform.position.ToVector2();
        }

        public override void UpdateState(PlayerController player)
        {
            if (_rotated >= TotalAngle)
            {
                Debug.Log("Sword has swung a full 90 degrees");
                player.stateMachine.SwitchState(new PlayerIdleState());
            }
            
            var delta = (TotalAngle / Duration) * Time.deltaTime;
            _currentWeapon.transform.RotateAround(_pivot, Vector3.forward, delta);
            _rotated += delta;
            Debug.Log("Rotated: " + _rotated);
        }

        public override void ExitState(PlayerController player)
        {
            GameObject.Destroy(_currentWeapon);
            _currentWeapon = null;
            // SheatheWeapon(player);
        }
        
        
        public void HandleAttack(PlayerController player)
        {
            if (Time.time - player.lastAttackTime < player.attackCooldown) { return; }
        
            //player.animator.SetTrigger(Attack);
            
            // Implement actual attack logic here
            var hits = Physics2D.OverlapCircleAll(player.transform.position, 1.5f);
            foreach (var hit in hits)
            {
                /*if (hit.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage(10);
                }*/
            }
        
            player.lastAttackTime = Time.time;
        }
        
        public void DrawWeapon(PlayerController player) => player.weaponObject.SetActive(true);
        public void SheatheWeapon(PlayerController player) => player.weaponObject.SetActive(false);
    }
}