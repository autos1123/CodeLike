using UnityEngine;

public class PlayerAnimationData
{
    private string moveParameterName = "isMoving";
    private string jumpParameterName = "isJumping";
    private string attackParameterName = "attack";
    private string deadParameterName = "isDead";

    public int MoveParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DeadParameterHash { get; private set; }

    public void Initialize()
    {
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        DeadParameterHash = Animator.StringToHash(deadParameterName);
    }
}
