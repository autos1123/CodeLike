using UnityEngine;
using System.Collections;

public class PlayerHPRunnerTest:MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private PlayerCondition playerCondition;

    [SerializeField] private float testDamage = 10f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => playerController.PlayerCondition != null);
        playerCondition = playerController.PlayerCondition;
    }

    private void Update()
    {
        if(playerCondition == null) return;

        if(Input.GetKeyDown(KeyCode.K))
        {
            playerCondition.TakenDamage(testDamage);
        }
    }
}
