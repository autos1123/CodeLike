using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가

[CreateAssetMenu(fileName = "NewTutorialInputStep", menuName = "Tutorial/Input Step")]
public class TutorialInputStep : TutorialStep
{
    [Header("입력 스텝 설정")]
    [TextArea]
    public string inputHintMessage; // 이 스텝에서 표시할 힌트 메시지

    private bool _isMonitoringInput = false;

    public override void Activate()
    {
        base.Activate();
        Debug.Log($"튜토리얼 입력 스텝 '{QuestDescription}' 활성화. 힌트: '{inputHintMessage}'");

        // UI 힌트 표시
        UIManager.Instance.ShowContextualHint(inputHintMessage);

        // 입력 감지 시작
        _isMonitoringInput = true;
      }

    public override void Deactivate()
    {
        base.Deactivate();
        _isMonitoringInput = false; // 입력 감지 중지
        Debug.Log($"튜토리얼 입력 스텝 '{QuestDescription}' 비활성화.");
    }

    // TutorialManager에서 호출하여 입력 감지 및 스텝 완료 여부를 확인
    public void CheckInput()
    {
        if (!_isMonitoringInput) return;

        // WASD 또는 방향키 입력 감지
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Debug.Log("이동 키 입력 감지! 튜토리얼 입력 스텝 완료.");
            _isMonitoringInput = false; 
            CompleteStep(); 
        }
    }
    
    public bool IsMonitoringInput()
    {
        return _isMonitoringInput;
    }
}
