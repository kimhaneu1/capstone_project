using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum GameObjects
    {
        Title,
        StartImage,
        TitleTextImage,
        Background,
        FillArea,
        Fill,
        Handle,
    }

    enum Sliders
    {
        LoadingSlider,
    }

    enum Texts
    {
        DisplayText,
        StatusText,
        StatusDetailText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        Managers.GPGS.OnStatusUpdated += OnGPGSStatusUpdated;
        Managers.Data.OnStatusUpdated += OnDataStatusUpdated;
        Managers.Data.OnDataLoaded += OnDataLoad;

        Managers.GPGS.GPGS_LogIn();

        return true;
    }

    private void OnGPGSStatusUpdated(string status)
    {
        GetText((int)Texts.StatusText).text = "구글 플레이 로그인 중";
        GetText((int)Texts.StatusDetailText).text = status;

        if (status == "로그인 상태: Success") SetLoadingProgressUI(0.1f);
    }
    private void OnDataStatusUpdated(string status)
    {
        GetText((int)Texts.StatusText).text = "게임 데이터 불러오는 중";
        GetText((int)Texts.StatusDetailText).text = status;
    }
    private void OnDataLoad()
    {
        Managers.Data.OnDataLoaded -= OnDataLoad;
        SetLoadingProgressUI(0.2f);
        StartLoadAssets();
    }

    private void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            GetText((int)Texts.StatusText).text = "게임 리소스 불러오는 중";
            GetText((int)Texts.StatusDetailText).text = $"{key}";

            float progress = 0.2f + ((float)count / totalCount) * 0.8f;
            SetLoadingProgressUI(progress);

            if (count == totalCount)
            {
                GetObject((int)GameObjects.Background).SetActive(false);

                GetText((int)Texts.StatusText).text = "";
                GetText((int)Texts.StatusDetailText).text = "";

                Managers.Sound.Init();

                GetText((int)Texts.DisplayText).text = $"Touch To Start";
                UI_DOTween.ApplyFadeMovement(GetText((int)Texts.DisplayText), 1.0f);

                GetObject((int)GameObjects.StartImage).BindEvent((evt) =>
                {
                    Debug.Log("EnterGameReadyScene");
                    Managers.Scene.LoadScene(EScene.GameReadyScene);
                });
            }
        });
    }

    private void SetLoadingProgressUI(float value)
    {
        Get<Slider>((int)Sliders.LoadingSlider).value = Mathf.Clamp01(value);
    }
}
