using System;
using UnityEngine;
[CreateAssetMenu(fileName = "NewTutorialEquipItemStep", menuName = "Tutorial/Equip Item Step")]
public class TutorialEquipItemStep : TutorialStep
{
    [Header("장비 착용 스텝 설정")]
    [TextArea]
    public string equipHintMessage; // 이 스텝에서 표시할 힌트 메시지

    private Action _cachedItemEquippedCallback; // 아이템 착용 이벤트를 받을 콜백 (인자 없음)

    public override void Activate()
    {
        base.Activate();
        
        TutorialManager.Instance.NotifyStepActivated(equipHintMessage, QuestDescription);
        
        _cachedItemEquippedCallback = HandleItemEquipped;
        GameEvents.OnItemEquipped += _cachedItemEquippedCallback; 
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        if (_cachedItemEquippedCallback != null)
        {
            GameEvents.OnItemEquipped -= _cachedItemEquippedCallback;
        }
    }
    
    private void HandleItemEquipped()
    {
        CompleteStep(); // 어떤 장비 아이템이든 착용되면 바로 스텝 완료
    }
}
