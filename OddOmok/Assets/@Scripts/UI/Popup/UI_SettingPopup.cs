public class UI_SettingPopup : UI_Popup
{
    enum GameObjects
    {
        Background,
        CancelButtonImage,
        Contents,
        BackgroundMusic,
        bgmSwitch,
        GameVibe,
        gvSwitch,
        PushAlert,
        paSwitch,
        NightAlert,
        naSwitch,
        GiftCode,
        FriendCode
    }

    enum Buttons
    {
        CancelButton,
        bgmSwitchButton,
        gvSwitchButton,
        paSwitchButton,
        naSwitchButton,
        fcCodeButton,
        ComplaintButton,
        CommunityButton,
    }

    enum Texts
    {
        bgmText,
        gvText,
        paText,
        naText,
        gcText,
        gcValueText,
        fcText,
        fcValueText,
        cmpText,
        cmmText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.CancelButton).gameObject.BindEvent((evt) => CloseSetting());
        GetButton((int)Buttons.bgmSwitchButton).gameObject.BindEvent((evt) => OnClickBGMSwitch());
        GetButton((int)Buttons.gvSwitchButton).gameObject.BindEvent((evt) => OnClickGVSwitch());
        GetButton((int)Buttons.paSwitchButton).gameObject.BindEvent((evt) => OnClickPASwitch());
        GetButton((int)Buttons.naSwitchButton).gameObject.BindEvent((evt) => OnClickNASwitch());

        Refresh();

        return true;
    }

    private void Refresh()
    {
        GetText((int)Texts.fcValueText).text = "clsrnzhem123";
    }

    public void OnClickBGMSwitch()
    {
        UI_DOTween.ApplySwitchToggle(GetButton((int)Buttons.bgmSwitchButton));
    }
    public void OnClickGVSwitch()
    {
        UI_DOTween.ApplySwitchToggle(GetButton((int)Buttons.gvSwitchButton));
    }
    public void OnClickPASwitch()
    {
        UI_DOTween.ApplySwitchToggle(GetButton((int)Buttons.paSwitchButton));
    }
    public void OnClickNASwitch()
    {
        UI_DOTween.ApplySwitchToggle(GetButton((int)Buttons.naSwitchButton));
    }

    private void CloseSetting()
    {
        ClosePopupUI();
    }
}
