using UnityEngine;

public class OpponentTurnState : IGameState
{
    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;

    public OpponentTurnState(GameStateMachine stateMachine, UI_GameScene uiGameScene)
    {
        _stateMachine = stateMachine;
        _uiGameScene = uiGameScene;
    }

    public void Enter()
    { 
        Debug.Log("상대방 차례");
        BoardManager.BM.OnStonePlaced += HandleStonePlace;
        _uiGameScene.ChangeTimerAlpha(0.5f);
        BoardManager.BM.StartTurnTimer();
    }
    public void Update()
    {
        BoardManager.BM.UpdateTimer();
    }
    public void Exit() 
    {
        BoardManager.BM.OnStonePlaced -= HandleStonePlace;
    }

    private void HandleStonePlace(int y, int x, BoardManager.StoneState state)
    {
        _uiGameScene.UpdateCell(y, x, state);
        _stateMachine.ChangeState(new CheckVictoryState(_stateMachine, _uiGameScene, y, x, state));
    }
}
