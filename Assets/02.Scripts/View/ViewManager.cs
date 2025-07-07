using System;
using UnityEngine;

public enum ViewModeType {
    View2D,
    View3D
}
/// <summary>
/// 2D ↔ 3D 시점 전환을 담당하는 싱글톤 매니저
/// </summary>
public class ViewManager : MonoSingleton<ViewManager>
{
    /// <summary>
    /// 현재 활성화된 시점 모드
    /// </summary>
    public ViewModeType CurrentViewMode { get; private set; } = ViewModeType.View3D;
    /// <summary>
    /// 시점이 변경될 때 호출되는 이벤트
    /// </summary>
    public event Action<ViewModeType> OnViewChanged;

    // 내부적으로 ViewCameraController 에서 세팅해 주는 값
    public bool IsTransitioning { get; set; }

    /// <summary>
    /// 지정한 시점 모드로 전환, 동일한 모드일 경우 무시
    /// </summary>
    /// <param name="mode">전환할 시점 모드</param>
    public void SwitchView(ViewModeType mode)
    {
        if (CurrentViewMode == mode) return;


        CurrentViewMode = mode;
        Debug.Log($"[ViewManager] 시점 전환: {mode}");

        OnViewChanged?.Invoke(mode);
    }
    /// <summary>
    /// 현재 시점 모드에서 반대 시점 모드로 전환 (2D ↔ 3D 토글)
    /// </summary>
    public void ToggleView()
    {
        var mode = CurrentViewMode == ViewModeType.View2D
            ? ViewModeType.View3D
            : ViewModeType.View2D;

        SwitchView(mode);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("전환");
            ToggleView();
        }        
    }
}
