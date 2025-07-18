using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTutorialKillMonsterStep", menuName = "Tutorial/Kill Monster Step")]
public class TutorialMonsterKillStep : TutorialStep
{
    [Header("몬스터 처치 스텝 설정")]
    [TextArea]
    public string killHintMessage; // 이 스텝에서 표시할 힌트 메시지
    
    private Action _cachedMonsterKilledCallback; 

    public override void Activate()
    {
        base.Activate();
        
        TutorialManager.Instance.NotifyStepActivated(killHintMessage, QuestDescription);
        
        _cachedMonsterKilledCallback = HandleMonsterKilled; 
        GameEvents.OnMonsterKilled += _cachedMonsterKilledCallback; 
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        if (_cachedMonsterKilledCallback != null)
        {
            GameEvents.OnMonsterKilled -= _cachedMonsterKilledCallback;
        }
    }
    
    private void HandleMonsterKilled()
    {
        CompleteStep(); // 몬스터가 한 번 죽으면 바로 스텝 완료
    }
}
