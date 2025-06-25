using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ViewCameraController : MonoBehaviour
{
    [Header("2D 시점 카메라 위치/회전")]
    public Vector3 position2D = new Vector3(0, 10, -10);
    public Vector3 rotation2D = new Vector3(30, 0, 0);

    [Header("3D 시점 카메라 위치/회전")]
    public Vector3 position3D = new Vector3(0, 5, -7);
    public Vector3 rotation3D = new Vector3(10, 0, 0);

    [Header("카메라 전환 시간")]
    public float transitionDuration = 1f;

    private void Start()
    {
        // 현재 시점에 맞춰 초기화
        ApplyView(ViewManager.Instance.CurrentViewMode, instant: true);

        // 시점 변경 시 자동 반응
        ViewManager.Instance.OnViewChanged += (mode) =>
        {
            ApplyView(mode, instant: false);
        };
    }

    private void ApplyView(ViewModeType mode, bool instant)
    {
        Vector3 targetPos = (mode == ViewModeType.View2D) ? position2D : position3D;
        Vector3 targetRot = (mode == ViewModeType.View2D) ? rotation2D : rotation3D;

        if (instant)
        {
            transform.position = targetPos;
            transform.eulerAngles = targetRot;
        }
        else
        {
            transform.DOMove(targetPos, transitionDuration);
            transform.DORotate(targetRot, transitionDuration);
        }
        
    }
}
