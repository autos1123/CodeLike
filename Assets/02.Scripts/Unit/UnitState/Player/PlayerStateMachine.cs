using UnityEngine;
public class PlayerStateMachine:UnitStateMachine
{
    public PlayerController Player { get; }

    public float MovementSpeed { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    public PlayerDeadState DeadState { get; private set; }


    public PlayerStateMachine(PlayerController player)
    {
        this.Player = player;
        ChangeState(new PlayerIdleState(this));

        if(Player.Data.TryGetCondition(ConditionType.MoveSpeed, out float value))
        {
            MovementSpeed = value;
        }
        else
        {
            Debug.LogError($"ConditionType MoveSpeed를 찾을 수 없습니다. 기본값으로 1.0f를 사용합니다.");
            MovementSpeed = 1.0f;
        }

        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        JumpState = new PlayerJumpState(this);
        AttackState = new PlayerAttackState(this);
        DeadState = new PlayerDeadState(this);

        ChangeState(IdleState);
    }
}
