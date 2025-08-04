using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallObstacleSpawner : ObstacleController
{
    [SerializeField] private WallObstacle[] obstacleContainers;
    [SerializeField] private float spawnInterval = 1f;
    private float spawnTimer = 0f;
    private Coroutine spawnCoroutine;

    private int endWallCount = 0;

    private void Start()
    {
        for(int i = 0; i < obstacleContainers.Length; i++)
        {
            obstacleContainers[i].Init(this);
            obstacleContainers[i].gameObject.SetActive(false);
        }
    }

    public override void PatternPlay()
    {
        isPatternEnd = false;

        if(spawnCoroutine != null)
        {
            ResetObstacle();
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(SetObstaclePos());
    }

    IEnumerator SetObstaclePos()
    {
        for(int i = 0; i < obstacleContainers.Length; i++)
        {
            obstacleContainers[i].gameObject.SetActive(true);

            while(true)
            {
                yield return null;
                if(isPlaying) spawnTimer += Time.deltaTime;

                if(spawnTimer >= spawnInterval)
                {
                    spawnTimer = 0f;
                    break;
                }
            }
        }
    }

    private void ResetObstacle()
    {
        for(int i = 0; i < obstacleContainers.Length; i++)
        {
            for(int j = 0; j < obstacleContainers[i].transform.childCount; j++)
            {
                obstacleContainers[i].gameObject.SetActive(false);
            }
        }
    }

    public void EndWallCountUp()
    {
        endWallCount++;
        if(endWallCount >= obstacleContainers.Length)
        {
            isPatternEnd = true;
            ResetObstacle();
            endWallCount = 0;
        }
    }
}
