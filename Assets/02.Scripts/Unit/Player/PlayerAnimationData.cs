using UnityEngine;

public class PlayerAnimationData
{
    private string idleParameterName = "Idle";
    private string moveParameterName = "isMoving";
    private string jumpParameterName = "isJumping";
    private string attackParameterName = "attack";
    private string deadParameterName = "isDead";
    private string knockBackParameterName = "isKnockBack";

    public int IdleParameterHash { get; private set; }
    public int MoveParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DeadParameterHash { get; private set; }
    public int KnockBackParameterHash { get; private set; }

    public PlayerAnimationData()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        DeadParameterHash = Animator.StringToHash(deadParameterName);
        KnockBackParameterHash = Animator.StringToHash(knockBackParameterName);
    }
}
