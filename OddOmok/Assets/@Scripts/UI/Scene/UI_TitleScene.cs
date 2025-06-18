using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum GameObjects
    {
        Title,
        StartImage,
    }

    enum Texts
    {
        TitleText,
        DisplayText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

        StartLoadAssets();

        return true;
    }

    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                Managers.Data.Init();

                GetText((int)Texts.DisplayText).text = $"Touch To Start";
                UI_DOTween.ApplyFadeMovement(GetText((int)Texts.DisplayText), 1.0f);

                GetObject((int)GameObjects.StartImage).BindEvent((evt) =>
                {
                    Debug.Log("ChangeScene");
                    Managers.Scene.LoadScene(EScene.GameReadyScene);
                });
            }

        });

        
    }
}
