using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialOpenInventoryStep", menuName = "Tutorial/Open Inventory Step")]
public class TutorialOpenInventoryStep:TutorialStep
{
    [Header("인벤토리 열기 스텝 설정")]
    [TextArea]
    public string openInventoryHintMessage;

    private Action _cachedInventoryOpenedCallback;

    public override void Activate()
    {
        base.Activate();

        TutorialManager.Instance.NotifyStepActivated(openInventoryHintMessage, QuestDescription);

        _cachedInventoryOpenedCallback = HandleInventoryOpened;
        GameEvents.OnInventoryOpened += _cachedInventoryOpenedCallback;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        if(_cachedInventoryOpenedCallback != null)
        {
            GameEvents.OnInventoryOpened -= _cachedInventoryOpenedCallback;
        }
    }

    private void HandleInventoryOpened()
    {
        CompleteStep();
    }
}
