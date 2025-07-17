using TMPro;
using UnityEngine;

/// <summary>
/// 마우스 위치에 따라 아이템 정보를 표시하는 툴팁 매니저 클래스.
/// 활성화/비활성화 및 텍스트 갱신, 위치 업데이트 기능을 제공.
/// </summary>
public class TooltipManager : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI descriptionText;
    [SerializeField]private RectTransform tooltipRect;

    [SerializeField]private GameObject tooltipGO;
    
    
    public bool IsVisible => tooltipGO != null && tooltipGO.activeSelf;
    private void Start()
    {
        // 모든 초기화 후 숨기기
        Hide(); 
    }
    
    public void RegisterTooltipUI(GameObject uiRoot)
    {
        tooltipGO = uiRoot;
        tooltipRect = tooltipGO.GetComponent<RectTransform>();
        descriptionText = tooltipGO.GetComponentInChildren<TextMeshProUGUI>();
    
        Hide();
    }

    /// <summary>
    /// 툴팁을 화면에 표시하고 지정한 위치로 이동
    /// </summary>
    /// <param name="description">표시할 아이템 설명 텍스트</param>
    /// <param name="position">마우스 위치</param>
    public void Show(string description, Vector2 position)
    {
        if (tooltipGO == null || descriptionText == null || tooltipRect == null)
        {
            Debug.LogWarning("[TooltipManager] 툴팁 UI 컴포넌트가 제대로 초기화되지 않았습니다. Show 호출 실패.");
            return;
        }
        
        if (string.IsNullOrEmpty(description))
        {
            Hide();
            return;
        }

        descriptionText.text = description;
        tooltipGO.SetActive(true);
        UpdatePosition(position);
    }
    
    /// <summary>
    /// 툴팁의 화면 위치
    /// </summary>
    /// <param name="position">마우스 위치</param>
    public void UpdatePosition(Vector2 position)
    {
        if (tooltipRect == null || !IsVisible) return; // 툴팁이 보이지 않거나 RectTransform이 없으면 업데이트 안 함

        tooltipRect.position = position;
    }

    /// <summary>
    /// 툴팁 숨김
    /// </summary>
    public void Hide()
    {
        if(tooltipGO != null)
        {
            tooltipGO.SetActive(false);
        }
    }
}
