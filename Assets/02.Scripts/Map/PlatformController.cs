using System.Collections.Generic;
using UnityEngine;

public class PlatformController:MonoBehaviour
{ 
    [Tooltip("플레이어가 실제로 충돌하는 플랫폼 콜라이더 (isTrigger = false 상태여야 함)")]
    public Collider physicalCollider;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb;

        if(!other.CompareTag(TagName.Player)) return;
        if((rb = other.attachedRigidbody) == null) return;

        // 플레이어가 플랫폼 위에 있고 낙하 중일 때만 충돌 가능하게 설정
        if(other.transform.position.y > transform.position.y && rb.velocity.y < 0)
            physicalCollider.excludeLayers = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag(TagName.Player)) return;

        // 나가면 다시 충돌 가능하게 설정
        physicalCollider.excludeLayers = LayerMask.GetMask(LayerName.Player);
    }
}
