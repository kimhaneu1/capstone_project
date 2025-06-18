using DG.Tweening;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        GameSideImage,
        BoardImage,
        OtherProfile,
        MyProfile,
        ChatInputField,
        OtherProfileImage,
        MyProfileImage,
        RecentWinRate,
    }

    enum Buttons
    {
        LaughButton,
        AngryButton,
        CryButton,
        ChatInputButton,
        PlaceButton,
        ItemButton,
    }

    enum Texts
    {
        OtherMMRText,
        MyMMRText,
        WrittenChat,
        Placeholder,
        ChatInputText,
        TurnTimerText,
        ItemCountText,
        PlaceText,
    }

    private Stone _playerStone;
    private Stone _opponentStone;
    private GameObject _lastSelectedCell;
    private int _x;
    private int _y;

    private int _maxItemUseCount = 2;
    private int _currentItemUseCount = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        InitializeBoard();

        GetButton((int)Buttons.ChatInputButton).gameObject.BindEvent((evt) => OnChatInputButtonClick());
        GetButton((int)Buttons.PlaceButton).gameObject.BindEvent((evt) => OnPlaceButtonClick());
        GetButton((int)Buttons.ItemButton).onClick.AddListener(() => Managers.Event.ItemButtonClicked());

        GetObject((int)GameObjects.OtherProfile).BindEvent((evt) => ToggleAllImagesAlpha(GetObject((int)GameObjects.OtherProfile)));
        GetObject((int)GameObjects.MyProfile).BindEvent((evt) => ToggleAllImagesAlpha(GetObject((int)GameObjects.MyProfile)));

        GetText((int)Texts.Placeholder).text = "여기에 채팅을 입력하세요";
        GetText((int)Texts.ChatInputText).text = "입력";
        GetText((int)Texts.PlaceText).text = "착 수";
        GetText((int)Texts.ItemCountText).text = $"{_maxItemUseCount - _currentItemUseCount}";

        return true;
    }

    private const int _boardSize = 13;
    private void InitializeBoard()
    {
        for (int y = 0; y < _boardSize;  y++)
        {
            for (int x = 0; x < _boardSize; x++)
            {
                GameObject go = new GameObject { name = $"Cell({y}, {x})" };
                go.transform.SetParent(GetObject((int)GameObjects.BoardImage).transform, false);
                go.AddComponent<Image>();
                go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                go.BindEvent((evt) => Managers.Event.CellClicked(go));
            }
        }
    }

    private bool isTransparent = false;
    private void ToggleAllImagesAlpha(GameObject gameObject)
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>(includeInactive: true);

        foreach (Image img in images)
        {
            Color color = img.color;
            color.a = isTransparent ? 1f : 0f;
            img.color = color;
        }

        isTransparent = !isTransparent;
    }

    public void SetStoneUI(string opponentStoneName)
    {
        _playerStone = Managers.Resource.Load<Stone>(GameServerManager.GameServer.selectedStoneName);
        _opponentStone = Managers.Resource.Load<Stone>(opponentStoneName);

        bool isBlack = GameServerManager.GameServer.GetPlayerType() == PlayerCharacter.PlayerType.BlackPlayer;

        Image myProfileImage = GetObject((int)GameObjects.MyProfileImage).GetComponent<Image>();
        Image myProfile = GetObject((int)GameObjects.MyProfile).GetComponent<Image>();
        Image otherProfileImage = GetObject((int)GameObjects.OtherProfileImage).GetComponent<Image>();
        Image otherProfile = GetObject((int)GameObjects.OtherProfile).GetComponent<Image>();

        myProfileImage.sprite = isBlack ? _playerStone.blackStoneImage : _playerStone.whiteStoneImage;
        myProfileImage.color = Color.white;
        myProfile.color = isBlack ? Color.black : Color.white;

        otherProfileImage.sprite = isBlack ? _opponentStone.whiteStoneImage : _opponentStone.blackStoneImage;
        otherProfileImage.color = Color.white;
        otherProfile.color = isBlack ? Color.white : Color.black;
    }

    public void SetMMRChangeUI(int opponentMMR)
    {
        var (myGain, myLoss) = Managers.Data.PredictMMRChanges(opponentMMR, false);
        var (opponentGain, opponentLoss) = Managers.Data.PredictMMRChanges(opponentMMR, true);

        GetText((int)Texts.MyMMRText).text =
            $"MMR {Managers.Data.dataSettings.MMR}\n<color=#4FC3F7>+{myGain}</color> <color=#EF5350>-{myLoss}</color>";

        GetText((int)Texts.OtherMMRText).text =
            $"MMR {opponentMMR}\n<color=#4FC3F7>+{opponentGain}</color> <color=#EF5350>-{opponentLoss}</color>";
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

        go.GetComponent<Image>().sprite = _playerStone.placeMarker;
        go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        _lastSelectedCell = go;
    }

    public Queue<Vector2Int> placedStoneQueue = new Queue<Vector2Int>();
    public void UpdateCell(int y, int x, BoardManager.StoneState state)
    {
        Transform cell = GetObject((int)GameObjects.BoardImage).transform.Find($"Cell({y}, {x})");
        
        Managers.Sound.Play(ESound.Effect, "Sound_SponeSlime");

        if (GameServerManager.GameServer.GetPlayerType() == PlayerCharacter.PlayerType.BlackPlayer)
        {
            cell.GetComponent<Image>().sprite = (state == BoardManager.StoneState.BlackStone)
                ? _playerStone.blackStoneImage
                : _opponentStone.whiteStoneImage;
        }
        else
        {
            cell.GetComponent<Image>().sprite = (state == BoardManager.StoneState.WhiteStone)
                ? _playerStone.whiteStoneImage
                : _opponentStone.blackStoneImage;
        }

        cell.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        placedStoneQueue.Enqueue(new Vector2Int(x, y));

        _lastSelectedCell = null;
    }
    public void UI_RemoveCell(int y, int x)
    {
        Transform cell = GetObject((int)GameObjects.BoardImage).transform.Find($"Cell({y}, {x})");
        DOTween.Kill(cell.GetComponent<Image>());
        cell.GetComponent<Image>().sprite = null;
        cell.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    public void ChangeTimerAlpha (float alpha)
    {
        GetText((int)Texts.TurnTimerText).color = new Color(1f, 1f, 1f, alpha);
    }

    public void UpdateTimerUI(float? remainingTime)
    {
        if (!remainingTime.HasValue) 
            return;

        GetText((int)Texts.TurnTimerText).text = $"{Mathf.FloorToInt(remainingTime.Value)}";
    }

    public void ShowGameResultPopup(PlayerCharacter.PlayerType winner)
    {
        UI_GameResultPopup popup = Managers.UI.ShowPopupUI<UI_GameResultPopup>();

        bool isWinner = (winner == GameServerManager.GameServer.GetPlayerType());
        if (isWinner)
        {
            if (winner == PlayerCharacter.PlayerType.BlackPlayer)
            {
                Managers.Data.dataSettings.BlackWins++;
            }
            else
            {
                Managers.Data.dataSettings.WhiteWins++;
            }
            Managers.Data.UpdateMMR(Managers.Data.opponentMMR, isWinner);

            popup.SetText("You Win!", Managers.Data.dataSettings.MMR);
        }
        else
        {
            popup.SetText("You Lose..", Managers.Data.dataSettings.MMR);
        }
    }

    private void OnChatInputButtonClick()
    {
        TMP_InputField inputField = GetObject((int)GameObjects.ChatInputField).GetComponent<TMP_InputField>();
        string inputText = inputField.text;

        if (string.IsNullOrWhiteSpace(inputText))
            return;

        inputField.text = "";

        BoardManager.BM.RPC_OnChatInputButtonClicked(GameServerManager.GameServer.Runner.LocalPlayer, inputText);
    }
    public void OnChatInputButtonClicked(bool isLocal, string message)
    {
        TMP_Text originalText = GetText((int)Texts.WrittenChat);
        GameObject cloneTextGO = Instantiate(originalText.gameObject, originalText.transform.parent);
        TMP_Text cloneText = cloneTextGO.GetComponent<TMP_Text>();
        if (isLocal) cloneText.text = "나) " + message;
        else cloneText.text = "적) " + message;
    }

    public void OnItemButtonClick(int y, int x)
    {
        Transform cell = GetObject((int)GameObjects.BoardImage).transform.Find($"Cell({y}, {x})");

        Image cellImage = cell.GetComponent<Image>();
        UI_DOTween.ApplyAlphaBlink(cellImage, 0f, 1f, 0.4f, 6);

        _currentItemUseCount++;
        GetText((int)Texts.ItemCountText).text = $"{_maxItemUseCount - _currentItemUseCount}";

        EnableItemButton(false);
    }

    public void EnableItemButton(bool enable)
    {
        if (_currentItemUseCount >= _maxItemUseCount)
        {
            GetButton((int)Buttons.ItemButton).interactable = false;
            return;
        }

        GetButton((int)Buttons.ItemButton).interactable = enable;
    }

    private void OnPlaceButtonClick()
    {
        if (_lastSelectedCell == null)
            return;

        BoardManager.StoneState state = (GameServerManager.GameServer.GetPlayerType() == PlayerCharacter.PlayerType.BlackPlayer)
        ? BoardManager.StoneState.BlackStone : BoardManager.StoneState.WhiteStone;

        BoardManager.BM.RPC_PlaceStone(_y, _x, state);
    }
}
