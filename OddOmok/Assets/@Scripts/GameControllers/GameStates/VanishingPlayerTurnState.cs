using UnityEngine;

public class VanishingPlayerTurnState : IGameState
{
    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;

    public VanishingPlayerTurnState(GameStateMachine stateMachine, UI_GameScene uiGameScene)
    {
        _stateMachine = stateMachine;
        _uiGameScene = uiGameScene;
    }

    public void Enter()
    {
        Debug.Log("사라지는 모드_내 차례");
        Managers.Event.OnCellClicked += HandleCellClicked;
        BoardManager.BM.OnStonePlaced += HandleStonePlace;
        Managers.Event.OnItemButtonClicked += HandleItemButtonClicked;
        _uiGameScene.EnableItemButton(true);
        _uiGameScene.ChangeTimerAlpha(1f);
        BoardManager.BM.StartTurnTimer();
    }
    public void Update()
    {
        BoardManager.BM.UpdateTimer();
    }
    public void Exit()
    {
        Managers.Event.OnCellClicked -= HandleCellClicked;
        BoardManager.BM.OnStonePlaced -= HandleStonePlace;
        Managers.Event.OnItemButtonClicked -= HandleItemButtonClicked;
        _uiGameScene.EnableItemButton(false);
    }

    private void HandleCellClicked(GameObject go)
    {
        _uiGameScene.CellClick(go);
    }
    private void HandleStonePlace(int y, int x, BoardManager.StoneState state)
    {
        _uiGameScene.UpdateCell(y, x, state);

        Vector2Int targetPos = _uiGameScene.placedStoneQueue.Dequeue();
        BoardManager.BM.RemoveCell(targetPos.y, targetPos.x);

        _stateMachine.ChangeState(new CheckVictoryState(_stateMachine, _uiGameScene, y, x, state));
    }
    private void HandleItemButtonClicked()
    {
        Vector2Int targetPos = _uiGameScene.placedStoneQueue.Peek();
        _uiGameScene.OnItemButtonClick(targetPos.y, targetPos.x);
    }
}
