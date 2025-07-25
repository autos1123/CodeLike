using UnityEngine;

public class PingPongObstacle : MovementObstacle
{
    [Header("Ping Pong Settings")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private float timer = 0f;

    protected override void Start()
    {
        base.Start();

        obstacleTransform.position = startPoint.position;
    }

    protected override void Movement()
    {
        timer += Time.deltaTime * speed;

        // 방향 벡터와 길이
        Vector3 direction = endPoint.position - startPoint.position;
        float distance = direction.magnitude;

        // 보간
        obstacleTransform.position = startPoint.position + direction.normalized * Mathf.PingPong(timer, distance);
    }
}
