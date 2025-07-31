using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealUI : UIBase
{
    public override string UIName => "HealUI";

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _healMessageText;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _exitButton;
    
    // 각 NPC 인스턴스 ID에 따라 고유한 회복 퍼센트를 저장할 딕셔너리
    private Dictionary<int, int> _healingPercentageByNPC = new Dictionary<int, int>();

    private GameObject _interactedNPC; 
    private int _currentNPCInstanceId; 

    public override void Open()
    {
        base.Open();
    }

    public void Open(GameObject npcObject)
    {
        _interactedNPC = npcObject;
        _currentNPCInstanceId = npcObject.GetInstanceID();
        
        // 현재 NPC의 회복 퍼센트가 캐시에 있는지 확인
        if (!_healingPercentageByNPC.TryGetValue(_currentNPCInstanceId, out int storedPercentage))
        {
            // 캐시에 없다면 새로운 랜덤 값 생성하여 저장
            storedPercentage = Random.Range(0, 101); // 0%에서 100% 사이의 랜덤 값 생성
            _healingPercentageByNPC[_currentNPCInstanceId] = storedPercentage;
            Debug.Log($"NPC({npcObject.name} ID:{_currentNPCInstanceId})에 대한 새로운 회복 퍼센트 계산: {storedPercentage}%");
        }
        else
        {
            Debug.Log($"NPC({npcObject.name} ID:{_currentNPCInstanceId})에 대한 기존 회복 퍼센트 사용: {storedPercentage}%");
        }
        
        // UI에 표시할 회복 퍼센트를 현재 NPC에 맞는 값으로 설정
        int _healingPercentage = storedPercentage; 
        
        UpdateUI(_healingPercentage); // 현재 NPC의 퍼센트로 UI 업데이트
        AddListeners(); 
        base.Open(); 
    }

    public override void Close()
    {
        RemoveListeners();
        
        // NPC가 치료 완료 상태인 경우에만 해당 NPC의 캐시된 회복 수치 정보를 제거
        if (GameManager.Instance != null && _interactedNPC != null && 
            GameManager.Instance.GetNpcInteractionProcessed(_interactedNPC))
        {
            _healingPercentageByNPC.Remove(_currentNPCInstanceId);
            Debug.Log($"NPC({_interactedNPC.name} ID:{_currentNPCInstanceId}) 치료 완료. 캐시된 회복 퍼센트 제거.");
        }
        
        base.Close();
    }

    private void UpdateUI(int percentage)
    {
        _healMessageText.text = $"현재 플레이어의 최대 체력의 <color=green>{percentage}%</color>만큼 체력을 회복합니다.\n진행하시겠습니까?";
    }

    private void AddListeners()
    {
        _okButton.onClick.AddListener(OnOkButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void RemoveListeners()
    {
        _okButton.onClick.RemoveListener(OnOkButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnOkButtonClick()
    {
        if (GameManager.Instance == null || GameManager.Instance.Player == null)
        {
            Debug.LogError("GameManager 또는 Player가 초기화되지 않았습니다.");
            return;
        }

        // PlayerController를 가져와서 Condition에 접근
        PlayerController playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("플레이어에게 PlayerController 스크립트가 없습니다.");
            return;
        }

        // BaseCondition 인스턴스에 접근
        BaseCondition playerCondition = playerController.Condition;
        if (playerCondition == null)
        {
            Debug.LogError("플레이어의 Condition이 초기화되지 않았습니다.");
            return;
        }
        // 현재 NPC의 캐시된 회복 퍼센트를 가져옴
        if (!_healingPercentageByNPC.TryGetValue(_currentNPCInstanceId, out int _healingPercentage))
        {
            Debug.LogError($"NPC({_interactedNPC.name} ID:{_currentNPCInstanceId})에 대한 회복 퍼센트 캐시를 찾을 수 없습니다.");
            Close();
            return;
        }
        
        float playerMaxHealth = playerCondition.GetTotalMaxValue(ConditionType.HP);
        
        // 회복할 체력 양 계산
        float healAmount = playerMaxHealth * (_healingPercentage / 100f);
        //회복하기전 체력
        float oldHealth = playerCondition.CurrentConditions[ConditionType.HP];
        
        playerCondition.Heal(healAmount);
        
        //실제 회복한 양
        float actualHealedAmount = playerCondition.CurrentConditions[ConditionType.HP] - oldHealth;

        UIManager.Instance.ShowConfirmPopup(
            $"체력을 {actualHealedAmount}만큼 회복했습니다",
            onConfirm: () => { },
            onCancel: null,
            confirmText: "확인"
        );
        
        // NPC가 치료 완료 상태임을 GameManager에 기록
        GameManager.Instance.SetNpcInteractionProcessed(_interactedNPC, true);
        Debug.Log($"{_interactedNPC.name} NPC로부터 치료를 완료했습니다.");

        Close(); 
    }

    private void OnExitButtonClick()
    {
        Close();
    }
}
