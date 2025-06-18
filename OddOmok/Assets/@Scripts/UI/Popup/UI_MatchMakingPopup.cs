using Fusion;
using UnityEngine;

public class UI_MatchMakingPopup : UI_Popup
{
    enum GameObjects
    {
        MatchingImage,
    }

    enum Buttons
    {
        MatchingCancelButton,
    }

    enum Texts
    {
        MatchingTimeText,
        MatchingCancelText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));
        
        Managers.Event.OnWaitingForMatch -= CancelImageOn;
        Managers.Event.OnWaitingForMatch += CancelImageOn;

        GetText((int)Texts.MatchingCancelText).text = "검색 취소";

        GetButton((int)Buttons.MatchingCancelButton).gameObject.BindEvent(async (evt) =>
        {
            ClosePopupUI();
            Managers.Event.OnWaitingForMatch -= CancelImageOn;

            await GameServerManager.GameServer.Runner.Shutdown();

            Debug.Log("매칭 취소");
        });

        CancelImageOff();

        return true;
    }

    private float elapsedTime = 0f;
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        GetText((int)Texts.MatchingTimeText).text = $"게임 찾는 중 {minutes}:{seconds:00}";
    }

    void CancelImageOn()
    {
        GetButton((int)Buttons.MatchingCancelButton).gameObject.SetActive(true);
    }

    void CancelImageOff()
    {
        GetButton((int)Buttons.MatchingCancelButton).gameObject.SetActive(false);
    }
}
