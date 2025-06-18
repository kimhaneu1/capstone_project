using Cysharp.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using static PlayerCharacter;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        WaitLoadAsync().Forget();

        return true;
    }

    private async UniTaskVoid WaitLoadAsync()
    {
         await UniTask.Delay(1000);

        Managers.Sound.Play(ESound.Bgm, "Sound_GameScene", 0.3f);
    }

    public override void Clear()
    {

    }
}
