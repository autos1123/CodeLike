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
        Debug.Log($"튜토리얼 몬스터 처치 스텝 '{QuestDescription}' 활성화. 현재 힌트: '{killHintMessage}'");
        
        
        TutorialManager.Instance.NotifyStepActivated(killHintMessage, QuestDescription);

        // 몬스터 처치 이벤트 구독
        _cachedMonsterKilledCallback = HandleMonsterKilled; 
        GameEvents.OnMonsterKilled += _cachedMonsterKilledCallback; 
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Debug.Log($"튜토리얼 몬스터 처치 스텝 '{QuestDescription}' 비활성화.");

        // 몬스터 처치 이벤트 구독 해제
        if (_cachedMonsterKilledCallback != null)
        {
            GameEvents.OnMonsterKilled -= _cachedMonsterKilledCallback;
        }
    }
    
    private void HandleMonsterKilled()
    {
        Debug.Log("튜토리얼 허수아비 처치 감지! 튜토리얼 스텝 완료.");
        CompleteStep(); // 몬스터가 한 번 죽으면 바로 스텝 완료
    }
}
