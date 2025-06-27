using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 카메라의 로컬 위치 및 회전을 통해 2D와 3D 시점 전환 및 레이어 렌더링 판별
/// </summary>
public class ViewCameraController:MonoBehaviour
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

    [Header("전환시 플레이어 추적")]
    private bool isTransitioning = false;
    [SerializeField] private Transform playerTransform;

    private Camera cam;
    private int layerMask;

    /// <summary>
    /// 시작 시 현재 시점 모드에 맞게 카메라를 설정후 해당 이벤트를 구독,레이어 마스크 판별
    /// </summary>
    private void Start()
    {
        cam = GetComponent<Camera>();
        if(cam == null)
        {
            Debug.LogError("[ViewCameraController] Camera 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        ApplyView(ViewManager.Instance.CurrentViewMode);
        ViewManager.Instance.OnViewChanged += ApplyView;
    }

    // /// <summary>
    // /// 전환시에도 플레이어 계속 바라봄
    // /// </summary>
    // private void LateUpdate()
    // {
    //     if (isTransitioning && playerTransform != null)
    //     {
    //         transform.LookAt(playerTransform.position);
    //     }
    // }

    /// <summary>
    /// 주어진 시점 모드에 따라 카메라 위치와 회전및 ForeWardBG레이어 렌더링 설정
    /// </summary>
    /// <param name="mode">적용할 시점 모드</param>
    private void ApplyView(ViewModeType mode)
    {
        // if (mode == ViewModeType.View2D)
        // {
        //     transform.DOLocalMove(position2D, transitionDuration).SetEase(Ease.OutQuad);
        //     transform.DOLocalRotate(rotation2D, transitionDuration).SetEase(Ease.OutQuad);
        //     cam.cullingMask &= ~layerToControl; 
        // }
        // else
        // {
        //     transform.DOLocalMove(position3D, transitionDuration).SetEase(Ease.OutQuad);
        //     transform.DOLocalRotate(rotation3D, transitionDuration).SetEase(Ease.OutQuad);
        //     cam.cullingMask |= layerToControl; 
        // }
        isTransitioning = true;

        Vector3 localTargetPos = (mode == ViewModeType.View2D) ? position2D : position3D;
        Vector3 worldTargetPos = transform.parent.TransformPoint(localTargetPos);

        Tween moveTween = transform.DOMove(worldTargetPos, transitionDuration)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                // Tween 진행 중에도 LookAt 적용
                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);
            })
            .OnComplete(() =>
            {
                isTransitioning = false;

                // 전환 완료 후 한 번 더 LookAt (보정용)
                if (playerTransform != null)
                    transform.LookAt(playerTransform.position);
            });

        // 렌더링 마스크 처리
        if (mode == ViewModeType.View2D)
            cam.cullingMask &= ~layerToControl;
        else
            cam.cullingMask |= layerToControl;
    }
}
