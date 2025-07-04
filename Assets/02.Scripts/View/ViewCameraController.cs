using UnityEngine;
using DG.Tweening;

/// <summary>
/// 플레이어 기준 로컬 위치를 기반으로 카메라의 2D/3D 시점을 전환하며,
/// 전환 도중에는 항상 플레이어를 바라보도록 회전.
/// 또한 특정 레이어의 렌더링을 시점에 따라 제어하고,
/// 시점 전환에 따라 HUD 애니메이션도 함께 제어.
/// </summary>
public class ViewCameraController : MonoBehaviour
{
    [Header("2D 시점 카메라 위치/회전")]
    public Vector3 position2D = new Vector3(67, 0, 0);
    public Vector3 rotation2D = new Vector3(0, -90, 0);

    [Header("3D 시점 카메라 위치/회전")]
    public Vector3 position3D = new Vector3(0, 5, -38);
    public Vector3 rotation3D = new Vector3(5, 0, 0);

    [Header("카메라 전환 시간")]
    public float transitionDuration = 1f;

    [Header("렌더링 제어할 레이어")]
    [SerializeField] private LayerMask layerToControl;

    [Header("전환 중 플레이어 추적")]
    private bool isTransitioning = false;
    [SerializeField] private Transform playerTransform;
    
    private ViewModeType previousMode;
    private Camera cam;
    private Tween moveTween;
    /// <summary>
    /// 시작 시 현재 ViewMode에 따라 카메라 위치를 초기화하고,
    /// ViewManager의 시점 전환 이벤트를 구독한다.
    /// </summary>

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("[ViewCameraController] Camera 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        previousMode = ViewManager.Instance.CurrentViewMode;
        ViewManager.Instance.OnViewChanged += ApplyView;
        ViewManager.Instance.SwitchView(previousMode);
        GameManager.Instance.onGameStateChange += OnStateChange;
    }

    /// <summary>
    /// 주어진 ViewMode(2D 또는 3D)에 따라 카메라의 위치를 전환하고,
    /// 전환 도중에는 매 프레임 플레이어를 바라보도록 한다.
    /// 렌더링 레이어를 활성화/비활성화하며, HUD 애니메이션도 트리거한다.
    /// </summary>
    /// <param name="mode">적용할 시점 모드 (2D 또는 3D)</param>
    private void ApplyView(ViewModeType mode)
    {
        isTransitioning = true;

        Vector3 localTargetPos = (mode == ViewModeType.View2D) ? position2D : position3D;
        Vector3 worldTargetPos = transform.parent.TransformPoint(localTargetPos);
        
        HUDAnimator hudAnimator = FindObjectOfType<HUDAnimator>();
        hudAnimator?.StartShift(previousMode, mode, transitionDuration);

        moveTween = transform.DOMove(worldTargetPos, transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                GameManager.Instance.setState(GameState.ViewChange);
            })
            .OnUpdate(() =>
            {
                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);
            })
            .OnComplete(() =>
            {
                isTransitioning = false;

                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);

                // 전환 완료 후 HUD 복귀
                hudAnimator?.ReturnToOriginal(transitionDuration - 0.5f);  // 복귀 시간은 약간 짧게
                GameManager.Instance.setState(GameState.Play);
            });

        // 레이어 렌더링 마스크 설정 (2D에서는 비활성화, 3D에서는 활성화)
        if (mode == ViewModeType.View2D)
            cam.cullingMask &= ~layerToControl;
        else
            cam.cullingMask |= layerToControl;
        
        previousMode = mode;
    }
    
    void OnStateChange()
    {
        if(moveTween.IsComplete()) return;

        if(GameManager.Instance.curGameState == GameState.Stop)
        {
            moveTween.Pause();
        }
        else
        {
            moveTween.Play();
        }
    }
}