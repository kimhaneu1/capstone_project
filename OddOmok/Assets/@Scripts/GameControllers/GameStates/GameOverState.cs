using UnityEngine;

public class GameOverState : IGameState
{
    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;
    private BoardManager.StoneState _winner;

    public GameOverState(GameStateMachine stateMachine, UI_GameScene uiGameScene, BoardManager.StoneState state)
    {
        _stateMachine = stateMachine;
        _uiGameScene = uiGameScene;
        _winner = state;
    }

    public void Enter() 
    {
        Debug.Log($"게임 오버!\n{_winner} 승리!");
    }
    public void Exit() 
    {
    
    }
    public void Update() 
    { 
    
    }
}
