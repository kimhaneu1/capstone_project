using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UI_DOTween
{
    /// <summary>
    /// 확대 축소 애니메이션: 타이틀
    /// </summary>
    public static Tween ApplyScaleMovement(Transform target, float scaleAmount = 0.05f, float duration = 1.0f)
    {
        if (target == null) return null;

        // 현재 위치 저장
        Vector3 currentScale = target.localScale;

        // 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 원래 위치를 기준으로 위아래로 움직이는 시퀀스
        sequence
            .Append(target.DOScale(currentScale * (1 + scaleAmount), duration))
            .Append(target.DOScale(currentScale / (1 + scaleAmount), duration))
            .SetLoops(-1, LoopType.Yoyo);

        return sequence;
    }

    /// <summary>
    /// 상하 움직임 애니메이션: 서브타이틀
    /// </summary>
    public static Tween ApplyUpDownMovement(Transform target, float moveAmount = 10f, float duration = 1.0f)
    {
        if (target == null) return null;
        // 현재 위치 저장
        Vector3 originalPosition = target.position;

        // 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 원래 위치를 기준으로 위아래로 움직이는 시퀀스
        sequence
            .Append(target.DOMoveY(originalPosition.y + moveAmount, duration))
            .Append(target.DOMoveY(originalPosition.y - moveAmount, duration))
            .SetLoops(-1, LoopType.Yoyo);

        return sequence;
    }

    /// <summary>
    /// Fade효과 애니메이션
    /// </summary>
    public static Tween ApplyFadeMovement(TMP_Text target, float duration = 1.0f)
    {
        if (target == null) return null;

        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(target.DOFade(1f, duration / 100))
            .Append(target.DOFade(0f, duration))
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(UpdateType.Normal, true);

        return sequence;
    }

    /// <summary>
    /// Alpha 깜빡임 효과 애니메이션: 게임 내 아이템 사용
    /// </summary>
    public static Tween ApplyAlphaBlink(Image target, float minAlpha = 0f, float maxAlpha = 1f, float blinkSpeed = 0.5f, int blinkCount = 5)
    {
        if (target == null) return null;

        Sequence sequence = DOTween.Sequence();
        sequence.SetTarget(target);

        for (int i = 0; i < blinkCount; i++)
        {
            sequence
                .Append(target.DOFade(minAlpha, blinkSpeed / 2))
                .Append(target.DOFade(maxAlpha, blinkSpeed / 2));
        }

        // 마지막에 강제로 maxAlpha로 설정
        sequence.Append(target.DOFade(maxAlpha, 0.01f));

        return sequence;
    }

    /// <summary>
    /// 스위치 토글 - 0에서 24로 부드럽게 이동 + 배경변경
    /// </summary>
    public static Tween ApplySwitchToggle(Button target)
    {
        if(target == null) return null;

        Sequence sequence = DOTween.Sequence();
        sequence.SetTarget(target);

        sequence
            .Append(target.transform.DOMoveX(24, 5.0f));

        return sequence;
    } 

    /// <summary>
    /// 모든 DOTween 애니메이션 정지
    /// </summary>
    public static void CleanupAllTweens()
    {
        DOTween.KillAll();
    }

    /// <summary>
    /// 타겟의 모든 DOTween 애니메이션 정지
    /// </summary>
    public static void CleanupTweens(Transform target)
    {
        DOTween.Kill(target);
    }
}
