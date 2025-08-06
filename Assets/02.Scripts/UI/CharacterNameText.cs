using TMPro;
using UnityEngine;

public class CharacterNameText:MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;

    private void OnEnable()
    {
        if (nameText == null)
            nameText = GetComponent<TextMeshProUGUI>();

        if (GameManager.Instance == null || GameManager.Instance.Player == null) return;

        var playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        if (playerController == null || playerController.Condition == null || playerController.Condition.Data == null) return;

        nameText.text = playerController.Condition.Data.CharacterName;
    }
}
