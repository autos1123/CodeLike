using System;
using UnityEngine;

public enum ViewModeType {
    View2D,
    View3D
}
public class ViewManager : MonoSingleton<ViewManager>
{
    public ViewModeType CurrentViewMode { get; private set; }
    
    public event Action<ViewModeType> OnViewChanged;
    
    
    public void SwitchView(ViewModeType mode)
    {
        if (CurrentViewMode == mode) return;

        CurrentViewMode = mode;
        Debug.Log($"[ViewManager] 시점 전환: {mode}");

        OnViewChanged?.Invoke(mode);
    }

    public void ToggleView()
    {
        var mode = CurrentViewMode == ViewModeType.View2D
            ? ViewModeType.View3D
            : ViewModeType.View2D;

        SwitchView(mode);
    }
}
