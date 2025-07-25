using TMPro;
using UnityEngine;

public class ContextualUIHint : UIBase
{
    [SerializeField] private TextMeshProUGUI hintText; 
    
    public override string UIName => this.GetType().Name;

    
    public void SetHintText(string text)
    {
        if (hintText != null)
        {
            hintText.text = text;
        }
        else
        {
            Debug.LogWarning("ContextualUIHint: hintText가 할당되지 않았습니다. 인스펙터에서 할당해주세요.", this);
        }
    }
}
