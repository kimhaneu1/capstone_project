using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameServerManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private GameObject _playerCharacter;

    private PlayerRef _localPlayerRef;
    private PlayerCharacter.PlayerType _playerType;

    private static GameServerManager _gameServer;
    public static GameServerManager GameServer { get { Init(); return _gameServer; } }

    public string selectedStoneName = "Slime";

    public static void Init()
    {
        if (_gameServer == null)
        {
            Debug.Log("게임서버매니저 초기화");
            GameObject go = GameObject.Find("@GameServerManager");
            if (go == null)
            {
                go = new GameObject { name = "@GameServerManager" };
                go.AddComponent<GameServerManager>();
                go.AddComponent<NetworkRunner>();
                go.GetComponent<GameServerManager>()._playerCharacter = Managers.Resource.Load<GameObject>("@PlayerCharacter");
            }

            DontDestroyOnLoad(go);

            // 초기화
            _gameServer = go.GetComponent<GameServerManager>();
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined 호출");
        int playerCount = runner.ActivePlayers.Count();

        if (player == runner.LocalPlayer)
        {
            _localPlayerRef = player;
            _playerType = (playerCount == 1) ? PlayerCharacter.PlayerType.BlackPlayer : PlayerCharacter.PlayerType.WhitePlayer;
        }

        Debug.Log($"Player joined: {player.PlayerId} / Total players: {playerCount}");
        
        if (playerCount == 2)
        {
            Debug.Log("2 players joined. LoadGameScene!");
            LoadGameScene();
        }

        Managers.Event.WaitingForMatch();
    }

    public void LoadGameScene()
    {
        Debug.Log("LoadGameScene 호출");
        if (Runner.IsSceneAuthority)
        {
            Runner.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadDone 호출");
        NetworkObject localPlayer = runner.Spawn(_playerCharacter, Vector3.zero, Quaternion.identity, _localPlayerRef);
        runner.SetPlayerObject(_localPlayerRef, localPlayer);
        localPlayer.GetComponent<PlayerCharacter>().Type = _playerType;
        localPlayer.GetBehaviour<PlayerCharacter>().InvokeGameStartEvent();
    }

    public PlayerCharacter.PlayerType GetPlayerType() => _playerType;

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player Left: {player.PlayerId}");

        if (runner.ActivePlayers.Count() == 0)
            runner.Shutdown();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("OnShutdown 호출");
    }

    #region 미구현
    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }
    public void PlayerJoined(PlayerRef player)
    {

    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }
    
    #endregion
}
