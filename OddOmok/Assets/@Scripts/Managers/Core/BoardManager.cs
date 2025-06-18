using Fusion;
using System;
using UnityEngine;
using static PlayerCharacter;

public class BoardManager : NetworkBehaviour
{
    private static BoardManager _bM;
    public static BoardManager BM { get { return _bM; } }

    private GameStateMachine _stateMachine;
    private UI_GameScene _uiGameScene;

    public enum StoneState
    {
        Empty,
        BlackStone,
        WhiteStone,
    }

    private const int _boardSize = 13;

    [Networked, Capacity(_boardSize * _boardSize)]
    public NetworkArray<StoneState> OmokBoard { get; }

    public StoneState this[int y, int x]
    {
        get => GetStoneState(y, x);
        set => SetStoneState(y, x, value);
    }

    private int BoardIndex(int y, int x) => (y * _boardSize) + x;

    public StoneState GetStoneState(int y, int x) => OmokBoard.Get(BoardIndex(y, x));
    private void SetStoneState(int y, int x, StoneState state) => OmokBoard.Set(BoardIndex(y, x), state);

    [Networked] public int TurnCount { get; private set; }
    public PlayerCharacter.PlayerType CurrentTurnType => (TurnCount % 2 == 0)
    ? PlayerCharacter.PlayerType.BlackPlayer : PlayerCharacter.PlayerType.WhitePlayer;

    private void Awake()
    {
        _bM = this;
        _stateMachine = new GameStateMachine();
        _uiGameScene = GameObject.Find("UI_GameScene").GetComponent<UI_GameScene>();

        Managers.Event.OnGameStart -= OnGameStart;
        Managers.Event.OnGameStart += OnGameStart;
    }

    public override void FixedUpdateNetwork()
    {
        _stateMachine?.Update();
    }

    private void OnGameStart(PlayerType localType)
    {
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GameOver(StoneState winner)
    {
        if (!HasStateAuthority)
            return;

        _stateMachine.ChangeState(new GameOverState(_stateMachine, _uiGameScene, winner));
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
}
