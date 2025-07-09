using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : UIBase
{
    public override string UIName => "ConfirmPopup";

    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    public void Setup(string message, Action onConfirm, Action onCancel = null)
    {
        messageText.text = message;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() =>
        {
            Close(); // base.Close()
            onConfirm?.Invoke();
        });

        cancelButton.onClick.AddListener(() =>
        {
            Close();
            onCancel?.Invoke();
        });
    }
}
