using System;
using UnityEngine;

public class CheckVictoryState : IGameState
{
    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;

    private int _lastY, _lastX;
    private BoardManager.StoneState _state;

    public CheckVictoryState(GameStateMachine stateMachine, UI_GameScene uiGameScene, int y, int x, BoardManager.StoneState state)
    {
        _stateMachine = stateMachine;
        _uiGameScene = uiGameScene;
        _lastY = y;
        _lastX = x;
        _state = state;
    }

    public void Enter()
    {
        Debug.Log("승리 요건 체크");

        if (BoardManager.BM.CheckVictory(_lastY, _lastX, _state))
        {
            BoardManager.BM.RPC_GameOver(_state);
            return;
        }

        if (BoardManager.BM.TryEnterVanishingMode())
            return;

        bool isVanishingMode = _stateMachine._previousState is VanishingPlayerTurnState or VanishingOpponentTurnState;

        _stateMachine.ChangeState(
            isVanishingMode
            ? (_stateMachine._previousState is VanishingPlayerTurnState
                ? new VanishingOpponentTurnState(_stateMachine, _uiGameScene)
                : new VanishingPlayerTurnState(_stateMachine, _uiGameScene))
            : (_stateMachine._previousState is PlayerTurnState
                ? new OpponentTurnState(_stateMachine, _uiGameScene)
                : new PlayerTurnState(_stateMachine, _uiGameScene))
        );
    }
    public void Update()
    {

    }
    public void Exit()
    {
    
    }
}
