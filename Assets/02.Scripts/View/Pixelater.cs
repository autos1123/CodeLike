
using UnityEngine;

/// <summary>
/// 화면을 픽셀화(Pixelate)하여 레트로 스타일 시각 효과를 제공하는 컴포넌트.
/// ViewManager의 ViewMode에 따라 2D 모드에서만 픽셀화를 적용.
/// </summary>
[ExecuteInEditMode]
public class Pixelater : MonoBehaviour
{
    [Range(1, 100)]
    public int _pixelate = 1;

    public bool _showGUI = true;
    /// <summary>
    /// 현재 픽셀화 효과 적용 여부 (2D 모드일 때만 true)
    /// </summary>
    private bool _pixelateActive = true;
    
    /// <summary>
    /// 초기화 시 ViewManager로부터 현재 ViewMode 정보를 받아 픽셀화 상태 초기 설정
    /// </summary>
    private void Awake()
    {
        TryUpdatePixelateState();
    }
    
    /// <summary>
    /// 활성화 시 ViewManager의 OnViewChanged 이벤트에 등록하여
    /// 시점 전환 시 픽셀화 상태를 실시간으로 반영
    /// </summary>
    private void OnEnable()
    {
        if (ViewManager.HasInstance)
        {
            ViewManager.Instance.OnViewChanged += OnViewModeChanged;
            TryUpdatePixelateState();
        }
    }
    
    /// <summary>
    /// 비활성화 시 이벤트 연결 해제
    /// </summary>
    private void OnDisable()
    {
        if (ViewManager.HasInstance)
            ViewManager.Instance.OnViewChanged -= OnViewModeChanged;
    }
    
    /// <summary>
    /// ViewManager로부터 시점 전환 알림을 받아 픽셀화 상태 갱신
    /// </summary>
    private void OnViewModeChanged(ViewModeType mode)
    {
        _pixelateActive = (mode == ViewModeType.View2D);
    }
    
    /// <summary>
    /// 현재 ViewMode 상태를 기반으로 픽셀화 적용 여부 판단
    /// </summary>
    private void TryUpdatePixelateState()
    {
        if (ViewManager.HasInstance)
            _pixelateActive = (ViewManager.Instance.CurrentViewMode == ViewModeType.View2D);
    }
    
    /// <summary>
    /// 카메라 렌더링 결과에 픽셀화 후처리 적용
    /// </summary>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_pixelateActive || _pixelate <= 1)
        {
            Graphics.Blit(source, destination);
            return;
        }
        source.filterMode = FilterMode.Point;
        RenderTexture resultTexture = RenderTexture.GetTemporary(source.width / _pixelate, source.height / _pixelate, 0, source.format);
        resultTexture.filterMode = FilterMode.Point;

        Graphics.Blit(source, resultTexture);
        Graphics.Blit(resultTexture, destination);
        RenderTexture.ReleaseTemporary(resultTexture);
    }
    
    /// <summary>
    /// 현재 픽셀화 강도 값을 화면에 GUI로 출력
    /// </summary>
    private void OnGUI()
    {
        if (!_showGUI) return;
        string text = $"Pixelate : {_pixelate,3}";

        Rect textRect = new Rect(60f, 60f, 440f, 100f);
        Rect boxRect = new Rect(40f, 40f, 460f, 120f);

        GUIStyle boxStyle = GUI.skin.box;
        GUI.Box(boxRect, "", boxStyle);

        GUIStyle textStyle = GUI.skin.label;
        textStyle.fontSize = 70;
        GUI.TextField(textRect, text, 50, textStyle);
    }
}