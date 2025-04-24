using UnityEngine;

public abstract class PlayerState
{
    public abstract void EnterState(PlayerController player);
    public abstract void UpdateState(PlayerController player);
    public abstract void ExitState(PlayerController player);
}

// State Machine Controller
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerController _playerController;
    public PlayerState CurrentState;

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        CurrentState = new PlayerIdleState();
        CurrentState.EnterState(_playerController);
    }

    void Update()
    {
        CurrentState.UpdateState(_playerController);
    }

    public void SwitchState(PlayerState newState)
    {
        if(CurrentState.GetType() == newState.GetType()) { return; }
        
        CurrentState.ExitState(_playerController);
        CurrentState = newState;
        newState.EnterState(_playerController);
    }
}
