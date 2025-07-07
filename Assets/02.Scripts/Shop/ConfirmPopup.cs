using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : MonoSingleton<ConfirmPopup>
{
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false); // 시작 시 비활성화
    }

    public static void Show(string message, Action onConfirm, Action onCancel = null)
    {
        if (!HasInstance)
        {
            Debug.LogError("ConfirmPopup 인스턴스가 없습니다.");
            return;
        }

        var popup = Instance;
        popup.gameObject.SetActive(true);
        popup.messageText.text = message;

        popup.confirmButton.onClick.RemoveAllListeners();
        popup.cancelButton.onClick.RemoveAllListeners();

        popup.confirmButton.onClick.AddListener(() =>
        {
            popup.gameObject.SetActive(false);
            onConfirm?.Invoke();
        });

        popup.cancelButton.onClick.AddListener(() =>
        {
            popup.gameObject.SetActive(false);
            onCancel?.Invoke();
        });
    }
}
