using TMPro;
using UnityEngine;

public class CharacterNameText:MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;

    private void OnEnable()
    {
        if(nameText == null)
            nameText = GetComponent<TextMeshProUGUI>();

        // 플레이어 이름 세팅 (예시)
        var playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        nameText.text = playerController.Condition.Data.CharacterName;

    }
}
