using Fusion;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static PlayerCharacter;

public class BoardManager : NetworkBehaviour
{
    private static BoardManager _bM;
    public static BoardManager BM { get { return _bM; } }

    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;

    [Networked] public int TurnCount { get; private set; }
    public PlayerCharacter.PlayerType CurrentTurnType => (TurnCount % 2 == 0)
    ? PlayerCharacter.PlayerType.BlackPlayer : PlayerCharacter.PlayerType.WhitePlayer;

    [Networked] public TickTimer TurnTimeLimit { get; private set; }

    private const int _boardSize = 13;
    private const float _timeLimitSeconds = 30.9f;

    [Networked, Capacity(_boardSize * _boardSize)]
    public NetworkArray<StoneState> OmokBoard { get; }
    public enum StoneState
    {
        Empty,
        BlackStone,
        WhiteStone,
    }
    public StoneState this[int y, int x]
    {
        get => GetStoneState(y, x);
        set => SetStoneState(y, x, value);
    }

    private int BoardIndex(int y, int x) => (y * _boardSize) + x;

    public StoneState GetStoneState(int y, int x) => OmokBoard.Get(BoardIndex(y, x));
    private void SetStoneState(int y, int x, StoneState state) => OmokBoard.Set(BoardIndex(y, x), state);

    private void Awake()
    {
        _bM = this;
        _stateMachine = new GameStateMachine();
        _uiGameScene = GameObject.Find("UI_GameScene").GetComponent<UI_GameScene>();

        Managers.Event.OnGameStart -= OnGameStart;
        Managers.Event.OnGameStart += OnGameStart;
    }
    private void Update()
    {
        _stateMachine?.Update();
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    private void RPC_SetUserState(PlayerRef requester, string stoneName, int MMR, bool winner = false)
    {
        if (GameServerManager.GameServer.Runner.LocalPlayer == requester)
            return;

        _uiGameScene.SetStoneUI(stoneName);
        _uiGameScene.SetMMRChangeUI(MMR);

        Managers.Data.UpdateGameCount();
        Managers.Data.UpdateMMR(MMR, winner);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
    public void RPC_OnChatInputButtonClicked(PlayerRef requester, string message)
    {
        bool isLocal = (GameServerManager.GameServer.Runner.LocalPlayer == requester);
        _uiGameScene.OnChatInputButtonClicked(isLocal, message);
    }

    private async void OnGameStart(PlayerType localType)
    {
        await UniTask.Delay(1000); // 여기에 나 vs 상대 대결 시작 애니메이션 추가

        Debug.Log("Game Start!");

        RPC_SetUserState(GameServerManager.GameServer.Runner.LocalPlayer, GameServerManager.GameServer.selectedStoneName, Managers.Data.dataSettings.MMR);

        if (localType == PlayerType.BlackPlayer)
            _stateMachine.ChangeState(new PlayerTurnState(_stateMachine, _uiGameScene));
        else
            _stateMachine.ChangeState(new OpponentTurnState(_stateMachine, _uiGameScene));
    }

    public event Action<int, int, StoneState> OnStonePlaced;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlaceStone(int y, int x, StoneState state)
    {
        if (OnStonePlaced == null) 
            return;

        if (HasStateAuthority)
        {
            SetStoneState(y, x, state);

            TurnCount++;
        }

        OnStonePlaced?.Invoke(y, x, state);
    }
    public void RemoveCell(int y, int x)
    {
        if (HasStateAuthority)
            SetStoneState(y, x, StoneState.Empty);

        _uiGameScene.UI_RemoveCell(y, x);
    }

    public void StartTurnTimer()
    {
        if (!HasStateAuthority)
            return;

        TurnTimeLimit = TickTimer.CreateFromSeconds(Runner, _timeLimitSeconds);
    }
    private float _timer = 1f;
    public void UpdateTimer()
    {
        _timer += Time.deltaTime;
        if (_timer < 1f) return;
        _timer = 0f;

        if (TurnTimeLimit.Expired(Runner) && HasStateAuthority)
        {
            Debug.Log("제한시간 초과!");
            PlayerCharacter.PlayerType winner = (CurrentTurnType == PlayerType.BlackPlayer)
            ? PlayerType.WhitePlayer : PlayerType.BlackPlayer;
            RPC_GameOver(winner);
        }

        _uiGameScene.UpdateTimerUI(TurnTimeLimit.RemainingTime(Runner));
    }

    public bool TryEnterVanishingMode()
    {
        if (!HasStateAuthority || TurnCount != 15)
            return false;

        RPC_EnterVanishingMode();

        return true;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_EnterVanishingMode()
    {
        _stateMachine.ChangeState(new EnterVanishingModeState(_stateMachine, _uiGameScene));
    }

    public void GameOver(PlayerCharacter.PlayerType player)
    {
        if (!HasStateAuthority)
            return;

        RPC_GameOver(player);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_GameOver(PlayerCharacter.PlayerType player)
    {
        _stateMachine.ChangeState(new GameOverState(_stateMachine, _uiGameScene, player));
    }

    public bool _isRematchRequested;
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RequestRematch(PlayerRef requester)
    {
        if (GameServerManager.GameServer.Runner.LocalPlayer == requester)
            return;

        Managers.UI.ShowPopupUI<UI_RematchPopup>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RespondRematch(PlayerRef responder, bool accepted)
    {
        if (accepted)
        {
            GameServerManager.GameServer.LoadGameScene();
            return;
        }

        if (GameServerManager.GameServer.Runner.LocalPlayer != responder)
            Managers.UI.ShowPopupUI<UI_NoticePopup>().SetText("상대가 재대결을\n거절했습니다");
    }

    public bool CheckVictory(int y, int x, StoneState state)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
        };

        foreach (var dir in directions)
        {
            int count = 1;

            count += CountStone(y, x, dir.x, dir.y, state);
            count += CountStone(y, x, -dir.x, -dir.y, state);

            if (count >= 5)
                return true;
        }

        return false;
    }
    private int CountStone(int startY, int startX, int dirY, int dirX, StoneState state)
    {
        int count = 0;
        int y = startY + dirY;
        int x = startX + dirX;

        while (y >= 0 && y < _boardSize && x >= 0 && x < _boardSize && GetStoneState(y, x) == state)
        {
            count++;
            y += dirY;
            x += dirX;
        }

        return count;
    }

    private void OnDestroy()
    {
        Managers.Event.OnGameStart -= OnGameStart;
    }
}
