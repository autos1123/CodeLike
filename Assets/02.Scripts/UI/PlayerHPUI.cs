using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 HP UI 관리 (이벤트 기반, TryGetCondition 방식)
/// </summary>
public class PlayerHPUI:MonoBehaviour
{
    [SerializeField] private Image hpBarImage;
    [SerializeField] private PlayerController playerController;

    private PlayerCondition playerCondition;
    private float maxHP = 1f; // 0으로 나누기 방지용

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => playerController.PlayerCondition != null);

        playerCondition = playerController.PlayerCondition;

        if(!playerController.Data.TryGetCondition(ConditionType.HP, out float maxHp))
        {
            Debug.LogError("[PlayerHPUI] 최대 HP 정보를 가져올 수 없습니다. 기본값 1 사용.");
            maxHp = 1f;
        }

        playerCondition.OnHPChanged += UpdateHpUI;

        // 초기 FillAmount 설정
        UpdateHpUI(playerCondition.GetValue(ConditionType.HP), maxHp);
    }

    /// <summary>
    /// HP UI FillAmount 갱신
    /// </summary>
    private void UpdateHpUI(float currentHP, float maxHP)
    {
        if(hpBarImage == null)
            return;

        float ratio = maxHP > 0 ? currentHP / maxHP : 0f;
        hpBarImage.fillAmount = ratio;
    }

    private void OnDestroy()
    {
        if(playerCondition != null)
            playerCondition.OnHPChanged -= UpdateHpUI;
    }
}
