using UnityEngine;

/// <summary>
/// 플레이어 사망 상태
/// </summary>
public class PlayerDeadState:PlayerBaseState
{

    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void StateEnter()
    {
        base.StateEnter();
        if(player.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        player.enabled = false;

        StartAnimation(player.AnimationData.DeadParameterHash);
    }

    public override void StateExit()
    {
        base.StateExit();
        StopAnimation(player.AnimationData.DeadParameterHash); // 부활 등 리셋 상황 대비
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StatePhysicsUpdate()
    {
       base.StatePhysicsUpdate();
    }
}
