using UnityEngine;
using DG.Tweening;

/// <summary>
/// 카메라의 로컬 위치와 회전을 통해 2D 및 3D 시점 전환을 제어하고,
/// 특정 레이어 렌더링을 제어하며, 전환 중에는 플레이어를 바라보도록 한다.
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

    private Camera cam;

    /// <summary>
    /// 시작 시 현재 ViewMode에 따라 카메라를 초기 설정하고,
    /// ViewManager의 시점 전환 이벤트에 등록한다.
    /// </summary>
    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("[ViewCameraController] Camera 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        ApplyView(ViewManager.Instance.CurrentViewMode);
        ViewManager.Instance.OnViewChanged += ApplyView;
    }

    /// <summary>
    /// 주어진 ViewMode에 따라 카메라 위치 전환 및 렌더링 레이어 설정을 수행하고,
    /// 전환 중에는 플레이어를 향해 카메라를 회전시킨다.
    /// </summary>
    /// <param name="mode">적용할 시점 모드 (2D 또는 3D)</param>
    private void ApplyView(ViewModeType mode)
    {
        isTransitioning = true;

        Vector3 localTargetPos = (mode == ViewModeType.View2D) ? position2D : position3D;
        Vector3 worldTargetPos = transform.parent.TransformPoint(localTargetPos);

        transform.DOMove(worldTargetPos, transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                // 전환 중 실시간으로 플레이어를 바라보게 회전
                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);
            })
            .OnComplete(() =>
            {
                isTransitioning = false;

                // 전환 종료 후 정확히 한 번 LookAt 적용 (보정용)
                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);
            });

        // 레이어 렌더링 마스크 설정 (2D에서는 비활성화, 3D에서는 활성화)
        if (mode == ViewModeType.View2D)
            cam.cullingMask &= ~layerToControl;
        else
            cam.cullingMask |= layerToControl;
    }
}