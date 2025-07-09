using System.Collections.Generic;
using UnityEngine;

public class PlatformController:MonoBehaviour
{ 
    private float dropTimer = 0f; //충돌 타이머
    public float dropDuration = 0.5f; // 충돌 비활성화 시간

    [Tooltip("플레이어가 실제로 충돌하는 플랫폼 콜라이더 (isTrigger = false 상태여야 함)")]
    public Collider physicalCollider;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb;

        if(!other.CompareTag("Player")) return;
        if((rb = other.attachedRigidbody) == null) return;

        if(other.transform.position.y > transform.position.y)
            physicalCollider.excludeLayers = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        // 나가면 다시 충돌 가능하게 설정
        physicalCollider.excludeLayers = LayerMask.GetMask("Player");
    }
}
