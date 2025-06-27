using UnityEngine;
using DG.Tweening;
/// <summary>
/// 시점(ViewMode) 전환 시 HUD를 좌우로 이동시키는 전환 연출을 담당한다.
/// 시점 변경 방향에 따라 HUD의 이동 방향이 달라지며,
/// 전환 종료 시 원래 위치로 자연스럽게 복귀시킨다.
/// </summary>
public class HUDAnimator : MonoBehaviour
{
    public float shiftAmount = 40f;
    public Ease ease = Ease.OutQuad;
    
    
    private Vector3 originalPos;
    private Tween shiftTween;
    
    /// <summary>
    /// HUD의 초기 위치를 저장한다.
    /// </summary>
    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// ViewMode 전환 시 HUD를 왼쪽 또는 오른쪽으로 이동시켜 시점 전환 효과를 연출한다.
    /// 이동 방향은 이전 시점과 전환될 시점의 관계에 따라 결정된다.
    /// </summary>
    /// <param name="fromMode">전환 이전의 시점 모드</param>
    /// <param name="toMode">전환 이후의 시점 모드</param>
    /// <param name="transitionTime">HUD 이동에 소요될 시간</param>
    public void StartShift(ViewModeType fromMode, ViewModeType toMode, float transitionTime)
    {
        Vector3 shiftDir = Vector3.zero;

        if (fromMode == ViewModeType.View2D && toMode == ViewModeType.View3D)
            shiftDir = Vector3.left;
        else if (fromMode == ViewModeType.View3D && toMode == ViewModeType.View2D)
            shiftDir = Vector3.right;

        Vector3 targetPos = originalPos + shiftDir * shiftAmount;

        // 초기 이동 (전환 시간 동안 천천히 이동)
        shiftTween?.Kill();
        shiftTween = transform.DOLocalMove(targetPos, transitionTime).SetEase(ease);
    }

    /// <summary>
    /// HUD를 원래의 초기 위치로 부드럽게 복귀시킨다.
    /// </summary>
    /// <param name="duration">복귀에 걸리는 시간</param>
    public void ReturnToOriginal(float duration)
    {
        shiftTween?.Kill();
        transform.DOLocalMove(originalPos, duration).SetEase(ease);
    }
}
