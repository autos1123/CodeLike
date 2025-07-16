using System;

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
    public ViewModeType CurrentViewMode { get; private set; } 
    /// <summary>
    /// 시점이 변경될 때 호출되는 이벤트
    /// </summary>
    public event Action<ViewModeType> OnViewChanged;
    
    /// <summary>
    /// 싱글톤 초기화 시 초기 뷰 모드를 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // // 게임 시작 시 초기 뷰 모드를 명시적으로 3D로 설정
        // // 필요에 따라 View3D 또는 다른 값으로 변경 가능
        CurrentViewMode = ViewModeType.View2D;
    }

    /// <summary>
    /// 지정한 시점 모드로 전환, 동일한 모드일 경우 무시
    /// </summary>
    /// <param name="mode">전환할 시점 모드</param>
    public void SwitchView(ViewModeType mode)
    {
        if (CurrentViewMode == mode) return;


        CurrentViewMode = mode;

        OnViewChanged?.Invoke(mode);
    }
    /// <summary>
    /// 현재 시점 모드에서 반대 시점 모드로 전환 (2D ↔ 3D 토글)
    /// </summary>
    public void ToggleView()
    {
        var targetmode = CurrentViewMode == ViewModeType.View2D
            ? ViewModeType.View3D
            : ViewModeType.View2D;

        SwitchView(targetmode);
    }
}
