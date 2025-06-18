using UnityEngine;

public class GameOverState : IGameState
{
    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;
    private PlayerCharacter.PlayerType _winner;

    public GameOverState(GameStateMachine stateMachine, UI_GameScene uiGameScene, PlayerCharacter.PlayerType winner)
    {
        _stateMachine = stateMachine;
        _uiGameScene = uiGameScene;
        _winner = winner;
    }

    public void Enter() 
    {
        Debug.Log($"게임 오버!\n{_winner} 승리!");
        _uiGameScene.ShowGameResultPopup(_winner);
        Managers.Sound.Stop(Define.ESound.Bgm);
    }
    public void Exit() 
    {
    
    }
    public void Update() 
    { 
    
    }
}
