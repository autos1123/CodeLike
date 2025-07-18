using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialTakeItemStep", menuName = "Tutorial/Take Item Step")]
public class TutorialTakeItemStep : TutorialStep
{
    [Header("아이템 획득 스텝 설정")]
    [TextArea]
    public string HintMessage;
    
    private Action _cachedItemAcquiredCallback; 

    public override void Activate()
    {
        base.Activate();
        
        TutorialManager.Instance.NotifyStepActivated(HintMessage, QuestDescription);
        
        _cachedItemAcquiredCallback = HandleItemAcquired;
        GameEvents.OnItemTake += _cachedItemAcquiredCallback; 
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        if (_cachedItemAcquiredCallback != null)
        {
            GameEvents.OnItemTake -= _cachedItemAcquiredCallback;
        }
    }
    
    private void HandleItemAcquired()
    {
        CompleteStep();
    }
}
