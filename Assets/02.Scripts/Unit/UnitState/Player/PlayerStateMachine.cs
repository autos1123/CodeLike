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
        MovementSpeed = Player.Condition.GetValue(ConditionType.MoveSpeed);

        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        JumpState = new PlayerJumpState(this);
        AttackState = new PlayerAttackState(this);
        DeadState = new PlayerDeadState(this);

        ChangeState(IdleState);
    }
}
