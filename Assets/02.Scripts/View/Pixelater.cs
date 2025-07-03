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
    
    private float _currentPixelate = 0f;   // 현재 적용 중 강도
    private float _targetPixelate = 0f;    // 목표 강도
    public float _lerpSpeed = 5f;          // 보간 속도
    
    public bool _showGUI = true;
    
    
    /// <summary>
    /// 초기화 시 ViewManager로부터 현재 ViewMode 정보를 받아 픽셀화 상태 초기 설정
    /// </summary>
    private void Awake()
    {
        TryUpdatePixelateState();
    }
    
    /// <summary>
    /// 활성화 시 ViewMode 전환 이벤트에 등록하고 초기 상태 반영
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
    /// 프레임마다 현재 픽셀화 강도를 목표 값으로 부드럽게 보간
    /// </summary>
    private void Update()
    {
        _currentPixelate = Mathf.Lerp(_currentPixelate, _targetPixelate, Time.deltaTime * _lerpSpeed);
        
    }
    
    /// <summary>
    /// ViewMode 전환 시, 2D일 경우 픽셀화 강도 적용 / 3D일 경우 해제
    /// </summary>
    private void OnViewModeChanged(ViewModeType mode)
    {
        //_pixelateActive = (mode == ViewModeType.View2D);
        _targetPixelate = (mode == ViewModeType.View2D) ? _pixelate : 0f;
    }
    
    /// <summary>
    /// 현재 ViewMode에 맞게 초기 픽셀화 강도 설정
    /// </summary>
    private void TryUpdatePixelateState()
    {
        if (ViewManager.HasInstance)
        {
            var mode = ViewManager.Instance.CurrentViewMode;
            _targetPixelate = (mode == ViewModeType.View2D) ? _pixelate : 0f;
        }
    }
    /// <summary>
    /// 카메라 렌더링 결과에 픽셀화 효과를 적용 (RenderTexture 해상도 축소/확대로 표현)
    /// </summary>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_currentPixelate <= 0.99f)
        {
            Graphics.Blit(source, destination);
            return;
        }
        source.filterMode = FilterMode.Point;
        float safePixelate = Mathf.Max(1f, _currentPixelate);
        int width = Mathf.Max(1, Mathf.RoundToInt(source.width / safePixelate));
        int height = Mathf.Max(1, Mathf.RoundToInt(source.height / safePixelate));

        RenderTexture resultTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
        resultTexture.filterMode = FilterMode.Point;

        Graphics.Blit(source, resultTexture);
        Graphics.Blit(resultTexture, destination);
        RenderTexture.ReleaseTemporary(resultTexture);
    }
    
    /// <summary>
    /// 현재 적용 중인 픽셀화 강도를 화면에 GUI로 출력 (디버그용)
    /// </summary>
    private void OnGUI()
    {
        if (!_showGUI) return;
        string text = $"Pixelate : {(int)_currentPixelate,3}";

        Rect textRect = new Rect(60f, 60f, 440f, 100f);
        Rect boxRect = new Rect(40f, 40f, 460f, 120f);

        GUIStyle boxStyle = GUI.skin.box;
        GUI.Box(boxRect, "", boxStyle);

        GUIStyle textStyle = GUI.skin.label;
        textStyle.fontSize = 70;
        GUI.TextField(textRect, text, 50, textStyle);
    }
}