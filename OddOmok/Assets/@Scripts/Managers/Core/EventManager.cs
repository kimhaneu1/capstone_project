using System;
using UnityEngine;
using static PlayerCharacter;

public class EventManager
{
    public event Action OnWaitingForMatch;
    public event Action<PlayerType> OnGameStart;
    public event Action<GameObject> OnCellClicked;

    public void WaitingForMatch()
    {
        OnWaitingForMatch?.Invoke();
    }
    public void GameStart(PlayerType type)
    {
        OnGameStart?.Invoke(type);
    }
    public void CellClicked(GameObject go)
    {
        OnCellClicked?.Invoke(go);
    }
}
