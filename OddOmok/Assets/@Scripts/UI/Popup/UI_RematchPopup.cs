using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_RematchPopup : UI_Popup
{
    enum GameObjects
    {
        OverlayImage,
        RematchPanelImage,
    }

    enum Buttons
    {
        AcceptButton,
        DeclineButton,
    }

    enum Texts
    {
        TitleText,
        NotificationText,
        AcceptText,
        DeclineText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetText((int)Texts.TitleText).text = "알 림";
        GetText((int)Texts.AcceptText).text = "수 락";
        GetText((int)Texts.DeclineText).text = "거 절";
        GetText((int)Texts.NotificationText).text = "상대가 재대결을\n신청했습니다";

        GetButton((int)Buttons.AcceptButton).gameObject.BindEvent((evt) => OnAcceptButtonClick());
        GetButton((int)Buttons.DeclineButton).gameObject.BindEvent((evt) => OnDeclineButtonClick());

        return true;
    }

    private void OnAcceptButtonClick()
    {
        ClosePopupUI();

        if (GameServerManager.GameServer.Runner.ActivePlayers.Count() < 2)
        {
            Managers.UI.ShowPopupUI<UI_NoticePopup>().SetText("상대가 떠났습니다");
            return;
        }

        BoardManager.BM.RPC_RespondRematch(GameServerManager.GameServer.Runner.LocalPlayer, true);
    }

    private void OnDeclineButtonClick()
    {
        ClosePopupUI();

        if (GameServerManager.GameServer.Runner.ActivePlayers.Count() == 2)
            BoardManager.BM.RPC_RespondRematch(GameServerManager.GameServer.Runner.LocalPlayer, false);
    }
}
