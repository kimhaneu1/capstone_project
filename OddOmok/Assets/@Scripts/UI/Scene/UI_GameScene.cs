using Fusion;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        GameSideImage,
        BoardImage,
        OtherStoneImage,
        MyStoneImage,
        RecentWinRate,
    }

    enum Buttons
    {
        LaughButton,
        AngryButton,
        CryButton,
        PlaceButton,
        ItemButton,
    }

    enum Texts
    {
        OtherRecentWinRateText,
        MyRecentWinRateText,
        PlaceText,
        ItemText,
    }

    [SerializeField]
    private Sprite _magicZoneCheck;
    [SerializeField]
    private Sprite _blackSlimeStone;
    [SerializeField]
    private Sprite _whiteSlimeStone;

    private GameObject _lastSelectedCell;
    private int _x;
    private int _y;
    private BoardManager.StoneState _state;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetText((int)Texts.PlaceText).text = "착 수";
        GetText((int)Texts.ItemText).text = "아이템";

        var _data = Managers.Data.TestDic[0];
        GetText((int)Texts.OtherRecentWinRateText).text = $"상대 백돌 승률\n{_data.WhiteWins}승{_data.WhiteGamePlayed - _data.WhiteWins}패({_data.WhiteWins * 100 / _data.WhiteGamePlayed}%)";
        GetText((int)Texts.MyRecentWinRateText).text = $"나의 흑돌 승률\n{_data.BlackWins}승{_data.BlackGamePlayed - _data.BlackWins}패({_data.BlackWins * 100 / _data.BlackGamePlayed}%)";

        InitializeBoard();

        GetButton((int)Buttons.PlaceButton).gameObject.BindEvent((evt) =>
        {
            if (_lastSelectedCell == null)
                return;

            _state = (GameServerManager.GameServer.GetPlayerType() == PlayerCharacter.PlayerType.BlackPlayer)
            ? BoardManager.StoneState.BlackStone : BoardManager.StoneState.WhiteStone;

            BoardManager.BM.RPC_PlaceStone(_y, _x, _state);
        });

        return true;
    }

    private const int _boardSize = 13;
    void InitializeBoard()
    {
        for (int y = 0; y < _boardSize;  y++)
        {
            for (int x = 0; x < _boardSize; x++)
            {
                GameObject go = new GameObject { name = $"Cell({y}, {x})" };
                go.transform.SetParent(GetObject((int)GameObjects.BoardImage).transform, false);
                go.AddComponent<Image>();
                go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                go.BindEvent((evt) =>
                {
                    Managers.Event.CellClicked(go);
                });
            }
        }
    }

    public void CellClick(GameObject go)
    {
        if (go.GetComponent<Image>().sprite != null)
            return;

        Match match = System.Text.RegularExpressions.Regex.Match(go.name, @"Cell\((\d+), (\d+)\)");

        _y = int.Parse(match.Groups[1].Value);  // x 값 추출
        _x = int.Parse(match.Groups[2].Value);  // y 값 추출

        Debug.Log($"셀 클릭됨: {go.name}");

        if (_lastSelectedCell != null && _lastSelectedCell != go)
        {
            _lastSelectedCell.GetComponent<Image>().sprite = null;
            _lastSelectedCell.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        }

        go.GetComponent<Image>().sprite = _magicZoneCheck;
        go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        _lastSelectedCell = go;
    }

    public Queue<Vector2Int> placedStoneQueue = new Queue<Vector2Int>();
    public void UpdateCell(int y, int x, BoardManager.StoneState state)
    {
        Transform cell = GetObject((int)GameObjects.BoardImage).transform.Find($"Cell({y}, {x})");

        cell.GetComponent<Image>().sprite = (state == BoardManager.StoneState.BlackStone)
        ? _blackSlimeStone : _whiteSlimeStone;
        cell.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        placedStoneQueue.Enqueue(new Vector2Int(x, y));

        _lastSelectedCell = null;
    }
    public void UI_RemoveCell(int y, int x)
    {
        Transform cell = GetObject((int)GameObjects.BoardImage).transform.Find($"Cell({y}, {x})");

        cell.GetComponent<Image>().sprite = null;
        cell.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }
}
