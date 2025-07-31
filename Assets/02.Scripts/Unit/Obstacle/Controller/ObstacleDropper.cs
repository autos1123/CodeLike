using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDropper : ObstacleController
{
    [SerializeField] private Transform obstacleContainer;
    private DropObstacle[] obstacles;
    private float resetTimer = 0f;
    private Coroutine resetCoroutine;

    private void Start()
    {
        obstacles = obstacleContainer.GetComponentsInChildren<DropObstacle>(true);
        for(int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].Init(this);
        }
    }

    public override void PatternPlay()
    {
        isPatternEnd = false;

        Vector3 pos = obstacleContainer.position;
        pos.z = 0;
        pos.x = GameManager.Instance.Player.transform.position.x;
        obstacleContainer.position = pos;

        if(resetCoroutine != null)
        {
            ResetObstacle();
            StopCoroutine(resetCoroutine);
        }

        SetObstaclePos();
        resetCoroutine = StartCoroutine(WaitForPatternEnd());
    }

    private void SetObstaclePos()
    {
        for(int i = 0; i < obstacles.Length; i++)
        {
            float z = Random.Range(-9, 9);
            Vector3 pos = obstacles[i].transform.localPosition;
            pos.z = z;
            obstacles[i].transform.localPosition = pos;
            obstacles[i].gameObject.SetActive(true);
        }
    }

    private void ResetObstacle()
    {
        for(int i = 0; i < obstacles.Length; i++)
        {
            Vector3 pos = obstacles[i].transform.localPosition;
            pos.y = 0;
            pos.z = 0;
            obstacles[i].transform.localPosition = pos;
            obstacles[i].gameObject.SetActive(false);
        }
    }

    IEnumerator WaitForPatternEnd()
    {
        while(true)
        {
            yield return null;
            if(isPlaying) resetTimer += Time.deltaTime;

            if(resetTimer >= 2.5f)
            {
                resetTimer = 0f;
                ResetObstacle();
                isPatternEnd = true; // 패턴이 끝났음을 알림
                break;
            }
        }
    }
}
