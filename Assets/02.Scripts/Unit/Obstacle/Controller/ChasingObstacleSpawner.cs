using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingObstacleSpawner :ObstacleController
{
    [SerializeField] private ChasingObstacle obstacle;
    [SerializeField] private float resetInterval = 5f; // Reset interval in seconds

    private float resetTimer = 0f;
    private Coroutine resetCoroutine;

    public override void PatternPlay()
    {
        isPatternEnd = false;
        obstacle.gameObject.SetActive(true);

        if(resetCoroutine != null)
        {
            ResetObstacle();
            StopCoroutine(resetCoroutine);
        }

        resetCoroutine = StartCoroutine(WaitResetObstacle());
    }

    private void ResetObstacle()
    {
        resetTimer = 0f;
        obstacle.gameObject.SetActive(false);
        isPatternEnd = true;
    }

    IEnumerator WaitResetObstacle()
    {
        while(true)
        {
            yield return null;
            if(isPlaying) resetTimer += Time.deltaTime;

            if(resetTimer >= resetInterval)
            {
                ResetObstacle();
                break;
            }
        }

        resetCoroutine = null;
    }
}
