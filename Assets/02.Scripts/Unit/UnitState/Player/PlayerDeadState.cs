using UnityEngine;

/// <summary>
/// 플레이어 사망 상태
/// </summary>
public class PlayerDeadState:IUnitState
{
    private PlayerController player;
    private PlayerStateMachine stateMachine;

    public PlayerDeadState(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public void StateEnter()
    {
        Debug.Log("PlayerDeadState 진입 - 플레이어 사망 처리");

        // Rigidbody 멈춤
        if(player.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // 입력 비활성화
        player.enabled = false;

        // 필요시 사망 애니메이션, 사운드, 게임오버 호출 등 가능
    }

    public void StateExit()
    {
        Debug.Log("PlayerDeadState 종료");
    }

    public void StateUpdate()
    {
        // 사망 상태에서는 아무 입력도 처리하지 않음
    }

    public void StatePhysicsUpdate()
    {
        // 물리 갱신도 없음
    }
}
