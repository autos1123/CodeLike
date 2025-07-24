using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 확인 및 취소 버튼이 있는 팝업 UI 클래스.
/// 메시지를 출력하고, 확인/취소에 따른 콜백을 실행
/// onCancel이 null인 경우, '확인' 버튼만 표시하고 가운데 정렬
/// </summary>
public class ConfirmPopup : UIBase
{
    public override string UIName => this.GetType().Name;

    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;
    public TextMeshProUGUI confirmButtonText;
    public TextMeshProUGUI cancelButtonText;

    /// <summary>
    /// 팝업 내용을 설정하고 버튼 콜백을 등록
    /// </summary>
    /// <param name="message">팝업에 표시할 메시지</param>
    /// <param name="onConfirm">확인 버튼 클릭 시 호출될 콜백</param>
    /// <param name="onCancel">취소 버튼 클릭 시 호출될 콜백 (null이면 버튼 숨김)</param>
    /// <param name="confirmText">확인 버튼에 표시할 텍스트 (기본값: "예")</param>
    /// <param name="cancelText">취소 버튼에 표시할 텍스트 (기본값: "아니오")</param>

    public void Setup(string message, Action onConfirm, Action onCancel = null,string confirmText = "예", string cancelText = "아니오")
    {
        messageText.text = message;

        confirmButtonText.text = confirmText;
        cancelButtonText.text = cancelText;
        
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        
        var layoutGroup = confirmButton.transform.parent.GetComponent<HorizontalLayoutGroup>();
        if(layoutGroup != null)
        {
            layoutGroup.enabled = true;
        }
        var rt = confirmButton.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, rt.anchorMin.y);
        rt.anchorMax = new Vector2(0f, rt.anchorMax.y);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        confirmButton.onClick.AddListener(() =>
        {
            Close(); // base.Close()
            onConfirm?.Invoke();
        });

        if (onCancel != null)
        {
            cancelButton.gameObject.SetActive(true);
            cancelButton.onClick.AddListener(() =>
            {
                Close();
                onCancel.Invoke();
            });
        }
        else
        {
            cancelButton.gameObject.SetActive(false); // 숨기기
            CenterConfirmButton(); // 가운데 정렬
        }
    }
    
    /// <summary>
    /// 확인 버튼만 표시되는 경우, 해당 버튼을 부모 내에서 가운데로 정렬
    /// 레이아웃 그룹이 존재하면 비활성화 처리
    /// </summary>
    private void CenterConfirmButton()
    {
        // 버튼 위치를 가운데로 옮기기
        var rt = confirmButton.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, rt.anchorMin.y);
        rt.anchorMax = new Vector2(0.5f, rt.anchorMax.y);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        // 필요하면 layout group 비활성화
        var layoutGroup = confirmButton.transform.parent.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }
    }
}
