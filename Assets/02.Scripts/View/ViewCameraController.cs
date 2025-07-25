using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// 플레이어 기준 로컬 위치를 기반으로 카메라의 2D/3D 시점을 전환하며,
/// 전환 도중에는 항상 플레이어를 바라보도록 회전.
/// 또한 특정 레이어의 렌더링을 시점에 따라 제어하고,
/// 시점 전환에 따라 HUD 애니메이션도 함께 제어.
/// </summary>
public class ViewCameraController:MonoBehaviour
{
    [SerializeField] private float lookPivotY = 1.0f;

    [Header("2D 시점 카메라 위치/회전")]
    public Transform pos2D;

    [Header("3D 시점 카메라 위치/회전")]
    public Transform pos3D;

    [Header("카메라 전환 시간")]
    public float transitionDuration = 1f;

    [Header("렌더링 제어할 레이어")]
    [SerializeField] private LayerMask layerToControl;
    
    [Header("카메라 속성 전환 비율")]
    [SerializeField, Range(0f, 1f)] private float orthographicEnableRatio = 0.8f; // 2D 전환 시 Orthographic 활성화 시점
    [SerializeField, Range(0f, 1f)] private float orthographicDisableRatio = 0.2f; // 3D 전환 시 Orthographic 비활성화 시점
    [SerializeField, Range(0f, 1f)] private float cullingMaskChangeRatio = 0.3f; // 컬링 마스크 변경 시점

    [Header("HUD 애니메이션 설정")]
    [SerializeField] private HUDAnimator hudAnimator;
    [SerializeField] private float hudReturnDelay = 0.5f; // HUD 복귀 애니메이션 지연 시간

    [Header("전환 중 플레이어 추적")]
    [SerializeField] private Transform playerTransform;

    private ViewModeType previousMode;
    private Camera cam;
    private Tween moveTween;
    private float currentLookAtY;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if(cam == null)
        {
            Debug.LogError("[ViewCameraController] Camera 컴포넌트를 찾을 수 없습니다.");
            return;
        }
    }

    /// <summary>
    /// 시작 시 현재 ViewMode에 따라 카메라 위치를 초기화하고,
    /// ViewManager의 시점 전환 이벤트를 구독한다.
    /// </summary>
    private void Start()
    {
        // HUDAnimator 참조 초기화 (인스펙터 할당 우선, 없으면 FindObjectOfType으로 찾음)
        if (hudAnimator == null)
        {
            hudAnimator = FindObjectOfType<HUDAnimator>();
            if (hudAnimator == null)
            {
                Debug.LogError("[ViewCameraController] 씬에서 HUDAnimator를 찾을 수 없습니다.");
            }
        }
        
        previousMode = ViewManager.Instance.CurrentViewMode;
        ViewManager.Instance.OnViewChanged += ApplyView;
        GameManager.Instance.onGameStateChange += OnStateChange;
        
        // 게임 시작 시 카메라를 초기 ViewMode에 따라 즉시 설정 (애니메이션 없이)
        SetCameraInstant(previousMode);
    }
    
    /// <summary>
    /// destroy 구독해제
    /// </summary>
    private void OnDestroy()
    {
        if (ViewManager.Instance != null)
        {
            ViewManager.Instance.OnViewChanged -= ApplyView;
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onGameStateChange -= OnStateChange;
        }
        // DOTween 시퀀스가 활성화되어 있다면 Kill (메모리 누수 방지)
        moveTween?.Kill();
    }
    
    /// <summary>
    /// 주어진 ViewMode(2D 또는 3D)에 따라 카메라의 위치를 전환하고,
    /// 전환 도중에는 매 프레임 플레이어를 바라보도록 한다.
    /// 렌더링 레이어를 활성화/비활성화하며, HUD 애니메이션도 트리거한다.
    /// </summary>
    /// <param name="mode">적용할 시점 모드 (2D 또는 3D)</param>
    private void ApplyView(ViewModeType mode)
    {
        // 기존 Tween이 있다면 강제로 종료 (새로운 전환 시작 전)
        moveTween?.Kill();

        Vector3 localTargetPos = (mode == ViewModeType.View2D) ? pos2D.localPosition : pos3D.localPosition;
        
        hudAnimator?.StartShift(previousMode, mode, transitionDuration);

        float startYForLookAt = currentLookAtY;
        float targetYForLookAt = (mode== ViewModeType.View2D)? pos2D.localPosition.y : lookPivotY;
        currentLookAtY = targetYForLookAt; // 최종 LookAt Y 위치 업데이트
        
        moveTween = transform.DOLocalMove(localTargetPos, transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                GameManager.Instance.setState(GameState.ViewChange);
            })
            .OnUpdate(() =>
            {
                if(playerTransform != null)
                {
                    float transitionPer = moveTween.ElapsedPercentage(); // DOTween에서 제공하는 진행도 사용
                    float yPos = Mathf.Lerp(startYForLookAt, targetYForLookAt, transitionPer);

                    transform.LookAt(playerTransform.position + Vector3.up * yPos);

                    UpdateCameraProperties(mode, transitionPer);
                }
            })
            .OnComplete(() =>
            {
                // 전환 완료 후 HUD 복귀
                hudAnimator?.ReturnToOriginal(Mathf.Max(0, transitionDuration - hudReturnDelay));
                GameManager.Instance.setState(GameState.Play);
            });

        previousMode = mode;
    }
    /// <summary>
    /// 카메라의 orthographic 및 cullingMask 속성을 전환 진행도에 따라 업데이트합니다.
    /// </summary>
    /// <param name="mode">현재 뷰 모드</param>
    /// <param name="transitionPercentage">전환 진행도 (0.0 ~ 1.0)</param>
    private void UpdateCameraProperties(ViewModeType mode, float transitionPercentage)
    {
        if (mode == ViewModeType.View2D)
        {
            if (transitionPercentage >= orthographicEnableRatio && !cam.orthographic)
            {
                cam.orthographic = true;
            }
            if (transitionPercentage >= cullingMaskChangeRatio && ((cam.cullingMask & layerToControl.value) != 0)) // .value 사용
            {
                cam.cullingMask &= ~layerToControl.value; // .value 사용
            }
        }
        else // View3D
        {
            if (transitionPercentage >= orthographicDisableRatio && cam.orthographic)
            {
                cam.orthographic = false;
            }
            if (transitionPercentage >= cullingMaskChangeRatio && ((cam.cullingMask & layerToControl.value) == 0)) // .value 사용
            {
                cam.cullingMask |= layerToControl.value; // .value 사용
            }
        }
    }
    /// <summary>
    /// 게임 상태 변경 시 카메라 전환 트윈을 일시 정지하거나 다시 재생합니다.
    /// </summary>
    /// <param name="newGameState">변경된 게임 상태</param>
    void OnStateChange()
    {
        // moveTween이 null이 아니며 활성화 상태일 때만 처리
        if (moveTween == null || !moveTween.IsActive()) return;
        
        if(GameManager.Instance.curGameState == GameState.Stop)
        {
            moveTween.Pause();
        }
        else
        {
            moveTween.Play();
        }
    }
    
    /// <summary>
    /// 카메라를 즉시 특정 뷰 모드 위치로 설정하고, 렌더링 설정을 적용
    /// 전환 애니메이션 없이 초기 설정에 사용
    /// </summary>
    /// <param name="mode">적용할 시점 모드 (2D 또는 3D)</param>
    public void SetCameraInstant(ViewModeType mode)
    {
        Vector3 localTargetPos = (mode == ViewModeType.View2D) ? pos2D.localPosition : pos3D.localPosition;
        transform.localPosition = localTargetPos;

        float targetYForLookAt = (mode == ViewModeType.View2D) ? pos2D.localPosition.y : lookPivotY;
        currentLookAtY = targetYForLookAt; // LookAt Y 위치 초기화
        
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform.position + Vector3.up * targetYForLookAt);
        }

        // 카메라 속성 즉시 적용
        cam.orthographic = (mode == ViewModeType.View2D);
        if (mode == ViewModeType.View2D)
        {
            cam.cullingMask &= ~layerToControl.value; // .value 사용
        }
        else
        {
            cam.cullingMask |= layerToControl.value; // .value 사용
        }
    }
}