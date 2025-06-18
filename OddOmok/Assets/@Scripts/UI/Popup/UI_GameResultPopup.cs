using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_GameResultPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        GameResultImage,
    }

    enum Buttons
    {
        RematchRequestButton,
        ExitMatchButton,
    }

    enum Texts
    {
        GameResultText,
        MMRText,
        RematchRequestText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetText((int)Texts.RematchRequestText).text = "재대결 신청";

        GetButton((int)Buttons.RematchRequestButton).gameObject.BindEvent((evt) => OnRematchRequestButtonClick());
        GetButton((int)Buttons.ExitMatchButton).gameObject.BindEvent((evt) => OnExitMatchButtonClick());

        return true;
    }

    public void SetText(string result, int MMR)
    {
        GetText((int)Texts.GameResultText).text = result;
        GetText((int)Texts.MMRText).text = $"MMR {MMR}점";
    }

    private async void OnExitMatchButtonClick()
    {
        await GameServerManager.GameServer.Runner.Shutdown();
        Debug.Log("ExitGameScene");
        Managers.Scene.LoadScene(EScene.GameReadyScene);
    }

    private void OnRematchRequestButtonClick()
    {
        if (GameServerManager.GameServer.Runner.ActivePlayers.Count() < 2)
        {
            Managers.UI.ShowPopupUI<UI_NoticePopup>().SetText("상대가 떠났습니다");
            return;
        }

        if (BoardManager.BM._isRematchRequested)
        {
            Managers.UI.ShowPopupUI<UI_NoticePopup>().SetText("이미 재대결을\n신청했습니다");
            return;
        }

        BoardManager.BM._isRematchRequested = true;
        Managers.UI.ShowPopupUI<UI_NoticePopup>().SetText("상대에게 재대결을\n신청했습니다");
        BoardManager.BM.RPC_RequestRematch(GameServerManager.GameServer.Runner.LocalPlayer);
    }
}
