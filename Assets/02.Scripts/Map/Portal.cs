using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Portal:MonoBehaviour
{
    public Transform destinationPoint;
    public Direction exitDirection;
    public float offsetDistance = 3f;
    public float cooldownDuration = 1f;

    private static readonly Dictionary<GameObject, float> lastTeleportTimes = new();

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player") || destinationPoint == null)
            return;

        float currentTime = Time.time;
        if(lastTeleportTimes.TryGetValue(other.gameObject, out float lastTime))
        {
            if(currentTime - lastTime < cooldownDuration)
                return;
        }

        // 위치 이동: 연결 방향 기반으로 오프셋 적용
        Vector3 offset = GetDirectionVector(exitDirection) * offsetDistance;
        other.transform.position = destinationPoint.position + offset;

        lastTeleportTimes[other.gameObject] = currentTime;

        // 반대편 포탈도 쿨타임 동기화
        Portal destPortal = destinationPoint.GetComponentInParent<Portal>();
        if(destPortal != null)
        {
            lastTeleportTimes[other.gameObject] = currentTime;
        }
    }

    private Vector3 GetDirectionVector(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => Vector3.zero
        };
    }
}
