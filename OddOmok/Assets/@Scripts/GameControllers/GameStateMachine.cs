using UnityEngine;

public interface IGameState
{
    void Enter();
    void Update();
    void Exit();
}
public class GameStateMachine
{
    private IGameState _currentState;

    public void ChangeState(IGameState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState?.Update();
    }
}
