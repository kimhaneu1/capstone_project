using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

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

    private bool isMatched;

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

        GetButton((int)Buttons.MatchingCancelButton).gameObject.BindEvent((evt) => OnMatchingCancelButtonClick());

        CancelImageOff();

        MatchingTimeout().Forget();

        return true;
    }

    private async UniTaskVoid MatchingTimeout()
    {
        await UniTask.Delay(10000);

        if (!isMatched)
        {
            Managers.Event.OnWaitingForMatch -= CancelImageOn;
            Managers.UI.CloseAllPopupUI();
            Managers.UI.ShowPopupUI<UI_NoticePopup>().SetText("서버 연결에\n실패했습니다");
            Debug.Log("10초 지나서 매칭 자동 취소");
        }
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
        isMatched = true;
        GetButton((int)Buttons.MatchingCancelButton).gameObject.SetActive(true);
    }

    void CancelImageOff()
    {
        isMatched = false;
        GetButton((int)Buttons.MatchingCancelButton).gameObject.SetActive(false);
    }

    private async void OnMatchingCancelButtonClick()
    {
        await GameServerManager.GameServer.Runner.Shutdown();
        Debug.Log("매칭 취소");
        Managers.Event.OnWaitingForMatch -= CancelImageOn;
        ClosePopupUI();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Managers.Event.OnWaitingForMatch -= CancelImageOn;
    }
}
