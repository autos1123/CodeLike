using UnityEngine;

/// <summary>
/// 플레이어 사망 상태
/// </summary>
public class PlayerDeadState:PlayerBaseState
{
    private static readonly int IsDeadHash = Animator.StringToHash("isDead");

    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        if(player.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        player.enabled = false;

        StartAnimation(IsDeadHash); 
    }

    public override void StateExit()
    {
        StopAnimation(IsDeadHash); // 부활 등 리셋 상황 대비
    }

    public override void StateUpdate()
    {

    }

    public override void StatePhysicsUpdate()
    {
       
    }
}
