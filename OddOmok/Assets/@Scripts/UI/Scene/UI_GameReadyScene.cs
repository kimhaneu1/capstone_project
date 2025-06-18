using Fusion;
using Fusion.Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameReadyScene : UI_Scene
{
    enum GameObjects
    {
        Background,
        StampImage,
        ProfileImage, 
    }

    enum Buttons
    {
        MailboxButton,
        OptionButton,
        PlayerPlateButton,
        GameStartButton,
        InventoryButton,
        RankingButton,
    }

    enum Texts
    {
        UserNameText,
        MMRText,
        MatchHistoryText,
        GameStartText,
        InventoryText,
        RankingText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        SetProfileImage(Managers.Resource.Load<Stone>(GameServerManager.GameServer.selectedStoneName));

        GetObject((int)GameObjects.ProfileImage).BindEvent((evt) => FlipStoneImage());

        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent((evt) => SetActiveGameStart());
        GetButton((int)Buttons.InventoryButton).gameObject.BindEvent((evt) => OpenInventory());
        GetButton((int)Buttons.RankingButton).gameObject.BindEvent((evt) => OpenRanking());
        GetButton((int)Buttons.PlayerPlateButton).gameObject.BindEvent((evt) => OpenInfo());
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent((evt) => OpenSetting());

        GetText((int)Texts.GameStartText).text = "게임시작";
        GetText((int)Texts.InventoryText).text = "인벤토리";
        GetText((int)Texts.RankingText).text = "랭킹";

        Refresh();

        return true;
    }

    private async void SetActiveGameStart()
    {
        if (Managers.UI.GetPopup<UI_MatchMakingPopup>() != null)
            return;

        Managers.UI.ShowPopupUI<UI_MatchMakingPopup>();

        NetworkRunner runner = GameServerManager.GameServer.GetComponent<NetworkRunner>();
        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            PlayerCount = 2,
            MatchmakingMode = MatchmakingMode.FillRoom,
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
        };
        await runner.StartGame(startGameArgs);
    }

    private void OpenInventory()
    {
        Managers.UI.ShowPopupUI<UI_InventoryPopup>();
    }
    private void OpenRanking()
    {
        Managers.UI.ShowPopupUI<UI_SelectRankingPopup>();
    }
    private void OpenInfo()
    {
        Refresh();
        Managers.UI.ShowPopupUI<UI_InfoPopup>();
    }
    private void OpenSetting()
    {
        Managers.UI.ShowPopupUI<UI_SettingPopup>();
    }

    private void Refresh()
    {
        var data = Managers.Data.dataSettings;

        int totalGames = data.BlackGamePlayed + data.WhiteGamePlayed;
        int totalWins = data.BlackWins + data.WhiteWins;
        int totalLosses = totalGames - totalWins;
        int winRate = totalGames > 0 ? (int)((float)totalWins / totalGames * 100) : 0;

        GetText((int)Texts.UserNameText).text = data.Name;
        GetText((int)Texts.MMRText).text = $"MMR {data.MMR}";
        GetText((int)Texts.MatchHistoryText).text =
            $"{totalGames}전 {totalWins}승 {totalLosses}패 ({winRate}%)";
    }

    private Stone _stone;
    private bool _isWhiteStone = true;
    private void FlipStoneImage()
    {
        GetObject((int)GameObjects.ProfileImage).GetComponent<Image>().sprite = _isWhiteStone ? _stone.blackStoneImage : _stone.whiteStoneImage;
        _isWhiteStone = !_isWhiteStone;
    }

    public void SetProfileImage(Stone selectedStone)
    {
        _stone = selectedStone;
        _isWhiteStone = true;

        GameObject profileObj = GetObject((int)GameObjects.ProfileImage);
        Image profileImage = profileObj.GetComponent<Image>();

        profileImage.sprite = _stone.whiteStoneImage;
        profileImage.color = Color.white;
    }
}
