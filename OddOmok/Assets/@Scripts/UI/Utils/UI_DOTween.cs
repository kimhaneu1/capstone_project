using UnityEngine;
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
