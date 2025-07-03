using System.Collections.Generic;
using UnityEngine;

public class PlatformController:MonoBehaviour
{
    [Tooltip("플레이어가 실제로 충돌하는 플랫폼 콜라이더 (isTrigger = false 상태여야 함)")]
    public Collider physicalCollider;

    private void OnTriggerStay(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        var input = other.GetComponent<PlayerInputHandler>();
        var rb = other.attachedRigidbody;
        if(input == null || rb == null) return;

        // ↓ 방향 입력 시: 통과 가능
        if(input.IsPressingDown())
        {
            physicalCollider.isTrigger = true;
        }
        // 낙하 중일 때: 충돌 활성화 (플랫폼 작동)
        else if(rb.velocity.y < -0.1f)
        {
            physicalCollider.isTrigger = false;
        }
        // 나머지 (정지, 점프 중): 통과 가능
        else
        {
            physicalCollider.isTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        // 나가면 다시 충돌 가능하게 설정
        physicalCollider.isTrigger = true;
    }
}
