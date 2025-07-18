using UnityEngine;
public enum TutorialInputType
{
    Move,     
    Jump,     
    Interact,
    Inventory,
    ViewChange
}
[CreateAssetMenu(fileName = "NewTutorialInputStep", menuName = "Tutorial/Input Step")]
public class TutorialInputStep : TutorialStep
{
    [Header("입력 스텝 설정")]
    [TextArea]
    public string inputHintMessage; // 이 스텝에서 표시할 힌트 메시지
    public TutorialInputType inputType = TutorialInputType.Move;

    private bool _isMonitoringInput = false;

    public override void Activate()
    {
        base.Activate();
        // UI 힌트 표시
        TutorialManager.Instance.NotifyStepActivated(inputHintMessage, QuestDescription);

        // 입력 감지 시작
        _isMonitoringInput = true;
      }

    public override void Deactivate()
    {
        base.Deactivate();
        _isMonitoringInput = false; // 입력 감지 중지
    }

    // TutorialManager에서 호출하여 입력 감지 및 스텝 완료 여부를 확인
    public void CheckInput()
    {
        if (!_isMonitoringInput) return;

        switch(inputType)
        {
            case TutorialInputType.Move:
                if(Input.GetAxis("Horizontal") != 0)
                    Complete();
                break;
            case TutorialInputType.Jump:
                if(Input.GetKeyDown(KeyCode.Space))
                    Complete();
                break;
            case TutorialInputType.Interact:
                if(Input.GetKeyDown(KeyCode.F))
                    Complete();
                break;
            case TutorialInputType.Inventory:
                if(Input.GetKeyDown(KeyCode.I))
                    Complete();
                break;
            case TutorialInputType.ViewChange:
                if(Input.GetKeyDown(KeyCode.V))
                    Complete();
                break;
        }
    }
    private void Complete()
    {
        _isMonitoringInput = false;
        CompleteStep();
    }
}
