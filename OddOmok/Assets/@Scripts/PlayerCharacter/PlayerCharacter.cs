using Fusion;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [Networked]
    public PlayerType Type { get; set; }

    public enum PlayerType
    {
        None,
        BlackPlayer,
        WhitePlayer,
    }

    public void InvokeGameStartEvent()
    {
        Debug.Log($"[Spawned] My PlayerType is {Type}");
        Managers.Event.GameStart(Type);
    }
}
