using Fusion;
using Fusion.Photon.Realtime;
using System.Linq;
using static Unity.Collections.Unicode;

public class UI_GameReadyScene : UI_Scene
{
    enum GameObjects
    {
        Background,
    }

    enum Buttons
    {
        GameStartButton,
    }

    enum Texts
    {
        GameStartText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindButtons(typeof(Buttons));
        BindTexts(typeof(Texts));

        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent((evt) =>
        {
            if (!Managers.UI.GetPopupStack().OfType<UI_MatchMakingPopup>().Any())
                SetActiveGameStart();
        });

        GetText((int)Texts.GameStartText).text = $"게임 시작";

        return true;
    }

    private async void SetActiveGameStart()
    {
        NetworkRunner runner = GameServerManager.GameServer.GetComponent<NetworkRunner>();

        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            PlayerCount = 2,
            MatchmakingMode = MatchmakingMode.FillRoom, // 빈자리가 있는 방을 우선적으로 찾음
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
        };

        OnClickShowMatchMakingPopup();

        await runner.StartGame(startGameArgs);
    }

    void OnClickShowMatchMakingPopup()
    {
        Managers.UI.ShowPopupUI<UI_MatchMakingPopup>();
    }
}
