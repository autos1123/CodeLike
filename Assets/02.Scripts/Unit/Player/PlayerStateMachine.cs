public class PlayerStateMachine:UnitStateMachine
{
    private PlayerController player;

    public PlayerStateMachine(PlayerController controller)
    {
        this.player = controller;
        ChangeState(new PlayerIdleState(player, this));
    }
}
