using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_SelectRankingPopup : UI_Popup
{
    enum GameObjects
    {
        OverlayImage,
        Background,
        CancelButtonImage,
    }

    enum Buttons
    {
        CancelButton,
        RankButton_MMRRanking,
        RankButton_TotalWins,
        RankButton_BlackWins,
        RankButton_WhiteWins,
    }

    enum Texts
    {
        RankingText,
        RankText_MMRRanking,
        RankText_TotalWins,
        RankText_BlackWins,
        RankText_WhiteWins,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetText((int)Texts.RankingText).text = "랭킹 선택";
        GetText((int)Texts.RankText_MMRRanking).text = "MMR 순위";
        GetText((int)Texts.RankText_TotalWins).text = "전체 승리 수";
        GetText((int)Texts.RankText_BlackWins).text = "흑돌 승리 수";
        GetText((int)Texts.RankText_WhiteWins).text = "백돌 승리 수";

        GetButton((int)Buttons.CancelButton).gameObject.BindEvent((evt) => ExitRankingSelect());
        GetButton((int)Buttons.RankButton_MMRRanking).onClick.AddListener(() => OpenGPGSRanking(GPGSManager.LeaderboardType.MMR));
        GetButton((int)Buttons.RankButton_TotalWins).onClick.AddListener(() => OpenGPGSRanking(GPGSManager.LeaderboardType.Total));
        GetButton((int)Buttons.RankButton_BlackWins).onClick.AddListener(() => OpenGPGSRanking(GPGSManager.LeaderboardType.Black));
        GetButton((int)Buttons.RankButton_WhiteWins).onClick.AddListener(() => OpenGPGSRanking(GPGSManager.LeaderboardType.White));

        return true;
    }

    private void ExitRankingSelect()
    {
        ClosePopupUI();
    }

    private void OpenGPGSRanking(GPGSManager.LeaderboardType type)
    {
        Managers.GPGS.ShowLeaderboardUI(type);
    }
}
