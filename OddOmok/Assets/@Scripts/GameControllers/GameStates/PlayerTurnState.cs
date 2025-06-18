using UnityEngine;

public class PlayerTurnState : IGameState
{
    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;

    public PlayerTurnState(GameStateMachine stateMachine, UI_GameScene uiGameScene)
    {
        _stateMachine = stateMachine;
        _uiGameScene = uiGameScene;
    }

    public void Enter() 
    { 
        Debug.Log("내 차례");
        Managers.Event.OnCellClicked += HandleCellClicked;
        BoardManager.BM.OnStonePlaced += HandleStonePlace;
    }
    public void Update()
    {

    }
    public void Exit() 
    {
        Managers.Event.OnCellClicked -= HandleCellClicked;
        BoardManager.BM.OnStonePlaced -= HandleStonePlace;
    }

    private void HandleCellClicked(GameObject go)
    {
        _uiGameScene.CellClick(go);
    }
    private void HandleStonePlace(int y, int x, BoardManager.StoneState state)
    {
        _uiGameScene.UpdateCell(y, x, state);
        _stateMachine.ChangeState(new CheckVictoryState(_stateMachine, _uiGameScene, y, x, state));
    }
}
