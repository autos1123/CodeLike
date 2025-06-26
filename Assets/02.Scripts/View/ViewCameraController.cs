using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 카메라의 로컬 위치 및 회전을 통해 2D와 3D 시점 전환 및 레이어 렌더링 판별
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

    private Camera cam;
    private int layerMask;
    
    /// <summary>
    /// 시작 시 현재 시점 모드에 맞게 카메라를 설정후 해당 이벤트를 구독,레이어 마스크 판별
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
    /// 주어진 시점 모드에 따라 카메라 위치와 회전및 ForeWardBG레이어 렌더링 설정
    /// </summary>
    /// <param name="mode">적용할 시점 모드</param>
    private void ApplyView(ViewModeType mode)
    {
        if (mode == ViewModeType.View2D)
        {
            transform.DOLocalMove(position2D, transitionDuration).SetEase(Ease.OutQuad);
            transform.DOLocalRotate(rotation2D, transitionDuration).SetEase(Ease.OutQuad);
            cam.cullingMask &= ~layerToControl; 
        }
        else
        {
            transform.DOLocalMove(position3D, transitionDuration).SetEase(Ease.OutQuad);
            transform.DOLocalRotate(rotation3D, transitionDuration).SetEase(Ease.OutQuad);
            cam.cullingMask |= layerToControl; 
        }
    }
}
