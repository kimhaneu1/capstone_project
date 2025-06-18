using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using static Define;

public class UI_NoticePopup : UI_Popup
{
    enum GameObjects
    {
        OverlayImage,
        NoticePanelImage,
    }

    enum Buttons
    {
        CheckButton,
    }

    enum Texts
    {
        TitleText,
        NotificationText,
        CheckText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetText((int)Texts.TitleText).text = "알 림";
        GetText((int)Texts.CheckText).text = "확인";

        GetButton((int)Buttons.CheckButton).gameObject.BindEvent((evt) => OnClickedCheckButton());

        return true;
    }

    public void SetText(string result)
    {
        GetText((int)Texts.NotificationText).text = result;
    }

    private void OnClickedCheckButton()
    {
        ClosePopupUI();
    }
}
