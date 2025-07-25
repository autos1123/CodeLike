using System;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTutorialOpenItemBox", menuName = "Tutorial/Open Item Box Step")]
public class TutorialOpenItemBoxStep : TutorialStep
{
    [Header("랜덤 아이템 UI 열기 스텝 설정")]
    [TextArea]
    public string uiOpenHintMessage; 

    private Action _cachedUIOpenedCallback; 

    public override void Activate()
    {
        base.Activate();
       
        TutorialManager.Instance.NotifyStepActivated(uiOpenHintMessage, QuestDescription);
        
        _cachedUIOpenedCallback = HandleRandomItemUIOpened;
        GameEvents.OnRandomItemUIOpened += _cachedUIOpenedCallback;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        if (_cachedUIOpenedCallback != null)
        {
            GameEvents.OnRandomItemUIOpened -= _cachedUIOpenedCallback;
        }
    }

    private void HandleRandomItemUIOpened()
    {
        CompleteStep();
    }
}
