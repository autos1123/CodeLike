using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI:MonoBehaviour
{
    [SerializeField] private Image hpBarImage; // Filled Image
    [SerializeField] private PlayerController playerController; // PlayerController 연결

    private PlayerCondition playerCondition;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => playerController.PlayerCondition != null);
        playerCondition = playerController.PlayerCondition;
    }

    private void Update()
    {
        if(playerCondition == null || hpBarImage == null)
            return;

        float ratio = playerCondition.GetCurrentHpRatio();
        hpBarImage.fillAmount = ratio;
    }
}
