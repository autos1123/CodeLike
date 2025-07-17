using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTutorialTriggerStep", menuName = "Tutorial/Trigger Step")]
public class TutorialTriggerStep : TutorialStep
{
    
    [Header("트리거 스텝 설정")]
    [TextArea]
    public string stepStartHintMessage;
    
    private TutorialTrigger targetTrigger;
    private System.Action<TutorialTrigger> _cachedTriggerCallback;
    
    [SerializeField] private string targetTriggerID;
    
    public override void Activate()
    {
        base.Activate();
        
        // ID로 씬에서 트리거 찾기
        targetTrigger = FindTriggerByID(targetTriggerID);
        
        if (targetTrigger == null)
        {
            Debug.LogError("TutorialTrigger 프리팹이 할당되지 않았습니다!");
            return;
        }
        
        targetTrigger.gameObject.SetActive(true);
        
        _cachedTriggerCallback = (trigger) => CompleteStep();
        targetTrigger.OnTriggerCompletedByPlayer += _cachedTriggerCallback;
        if (!string.IsNullOrEmpty(stepStartHintMessage)) // 메시지가 비어있지 않은 경우에만 표시
        {
            UIManager.Instance.ShowContextualHint(stepStartHintMessage);
            Debug.Log($"튜토리얼 트리거 스텝 '{QuestDescription}' 활성화. 초기 힌트: '{stepStartHintMessage}'");
        }
        Debug.Log($"튜토리얼 트리거 스텝 '{QuestDescription}' 활성화. 트리거 '{targetTrigger.name}' 생성.");
    }
    private TutorialTrigger FindTriggerByID(string id)
    {
        foreach (var trigger in GameObject.FindObjectsOfType<TutorialTrigger>(true)) 
        {
            if (trigger.GetTriggerID() == id)
                return trigger;
        }
        return null;
    }
    public override void Deactivate()
    {
        base.Deactivate();
        if (targetTrigger != null && _cachedTriggerCallback != null)
        {
            targetTrigger.OnTriggerCompletedByPlayer -= _cachedTriggerCallback;
        }
    
        targetTrigger.gameObject.SetActive(false);
        Debug.Log($"튜토리얼 트리거 스텝 비활성화. 목표 트리거 '{targetTrigger.name}' 비활성화됨.");

    }
}
