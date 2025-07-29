using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTutorialEventWaitStep", menuName = "Tutorial/Event Wait Step")]
public class TutorialEventWaitStep : TutorialStep
{
     [Header("Event Wait Settings")]
    [TextArea] public string waitMessage; // 이 스텝에서 보여줄 힌트
    
    // 어떤 이벤트를 기다릴지 UnityEvent로 설정하여 인스펙터에서 연결
    // GameEvents의 특정 Action을 기다릴 경우, 이 부분은 코드로 직접 구독해야 합니다.
    // 여기서는 GameEvents를 직접 구독하는 방식으로 진행하겠습니다.
    // UnityEvent는 에디터에서 다른 컴포넌트의 메서드를 연결할 때 유용합니다.

    public enum GameEventTypeToWait
    {
        None,
        OnMonsterKilled,
        OnPassiveItemUIOpened,
        OnPassiveItemTake,
        OnActiveItemUIOpened,
        OnActiveItemTake,
        OnActiveSkillUse,
        OnInventoryOpened,
        OnItemEquipped
    }

    public GameEventTypeToWait eventToWait;

    public override void Activate()
    {
        base.Activate();
        Debug.Log($"[TutorialStep] '{QuestDescription}' 스텝 활성화. 이벤트 대기 중: {eventToWait}");
        
        TutorialManager.Instance.NotifyStepActivated(waitMessage, QuestDescription);

        
        switch (eventToWait)
        {
            case GameEventTypeToWait.OnMonsterKilled:
                GameEvents.OnMonsterKilled += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnPassiveItemUIOpened:
                GameEvents.OnPassiveItemUIOpened += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnPassiveItemTake:
                GameEvents.OnPassiveItemTake += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnActiveItemUIOpened:
                GameEvents.OnActiveItemUIOpened += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnActiveItemTake:
                GameEvents.OnActiveItemTake += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnActiveSkillUse:
                GameEvents.OnActiveSkillUse += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnInventoryOpened:
                GameEvents.OnInventoryOpened += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnItemEquipped:
                GameEvents.OnItemEquipped += OnEventTriggered;
                break;
            default:
                Debug.LogWarning($"[TutorialEventWaitStep] '{QuestDescription}' 스텝: 기다릴 이벤트가 설정되지 않았습니다. 즉시 완료됩니다.");
                CompleteStep(); 
                break;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Debug.Log($"[TutorialStep] '{QuestDescription}' 스텝 비활성화.");
        
        switch (eventToWait)
        {
            case GameEventTypeToWait.OnMonsterKilled:
                GameEvents.OnMonsterKilled -= OnEventTriggered;
                break;
            case GameEventTypeToWait.OnPassiveItemUIOpened:
                GameEvents.OnPassiveItemUIOpened -= OnEventTriggered;
                break;
            case GameEventTypeToWait.OnPassiveItemTake:
                GameEvents.OnPassiveItemTake -= OnEventTriggered;
                break;
            case GameEventTypeToWait.OnActiveItemUIOpened:
                GameEvents.OnActiveItemUIOpened += OnEventTriggered;
                break;
            case GameEventTypeToWait.OnActiveItemTake:
                GameEvents.OnActiveItemTake -= OnEventTriggered;
                break;
            case GameEventTypeToWait.OnActiveSkillUse:
                GameEvents.OnActiveSkillUse -= OnEventTriggered;
                break;
            case GameEventTypeToWait.OnInventoryOpened:
                GameEvents.OnInventoryOpened -= OnEventTriggered;
                break;
            case GameEventTypeToWait.OnItemEquipped:
                GameEvents.OnItemEquipped -= OnEventTriggered;
                break;
        }
        UIManager.Instance.HideContextualHint();
    }

    private void OnEventTriggered()
    {
        Debug.Log($"[TutorialEventWaitStep] '{QuestDescription}' 스텝: 이벤트 ({eventToWait}) 감지. 스텝 완료.");
        CompleteStep();
    }
}
