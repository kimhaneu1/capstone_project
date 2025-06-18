public class UI_InfoPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        CancelButtonImage,
        ProfileContents,
        ProfileImage,
        PlayerStatsImage,
    }

    enum Buttons
    {
        CancelButton,
        EditButton,
    }

    enum Texts
    {
        UserNameText,
        MMRText,
        LabelText,
        PlayerStatsText,
        EditButtonText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.CancelButton).gameObject.BindEvent((evt) => CloseInfo());

        Refresh();

        return true;
    }

    private void Refresh()
    {
        var data = Managers.Data.dataSettings;

        int totalWins = data.BlackWins + data.WhiteWins;
        int totalGames = data.BlackGamePlayed + data.WhiteGamePlayed;
        int totalLosses = totalGames - totalWins;
        int totalWinRate = totalGames > 0 ? (int)((float)totalWins / totalGames * 100) : 0;

        int blackLosses = data.BlackGamePlayed - data.BlackWins;
        int blackWinRate = data.BlackGamePlayed > 0 ? (int)((float)data.BlackWins / data.BlackGamePlayed * 100) : 0;

        int whiteLosses = data.WhiteGamePlayed - data.WhiteWins;
        int whiteWinRate = data.WhiteGamePlayed > 0 ? (int)((float)data.WhiteWins / data.WhiteGamePlayed * 100) : 0;

        string totalStatsText = $"{totalWins}승 {totalLosses}패 ({totalWinRate}%)";
        string blackStatsText = $"{data.BlackWins}승 {blackLosses}패 ({blackWinRate}%)";
        string whiteStatsText = $"{data.WhiteWins}승 {whiteLosses}패 ({whiteWinRate}%)";

        GetText((int)Texts.UserNameText).text = data.Name;
        GetText((int)Texts.MMRText).text = $"MMR {data.MMR}";
        GetText((int)Texts.LabelText).text =
            "전적\n흑돌 전적\n백돌 전적";
        GetText((int)Texts.PlayerStatsText).text =
            $"{totalStatsText}\n{blackStatsText}\n{whiteStatsText}";
        GetText((int)Texts.EditButtonText).text = "프로필 편집하기";
    }

    private void CloseInfo()
    {
        ClosePopupUI();
    }
}
