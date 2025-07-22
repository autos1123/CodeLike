using UnityEngine;

public class PlayerAnimationData
{
    private string idleParameterName = "Idle";
    private string moveParameterName = "isMoving";
    private string jumpParameterName = "isJumping";
    private string attackCombo1ParameterName = "attack";
    private string attackCombo2ParameterName = "attack2";
    private string attackCombo3ParameterName = "attack3";
    private string deadParameterName = "isDead";
    private string knockBackParameterName = "isKnockBack";
    private string fallParameterName = "isFalling";
    private string landingParameterName = "Landing";

    public int IdleParameterHash { get; private set; }
    public int MoveParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int AttackCombo1ParameterHash { get; private set; }
    public int AttackCombo2ParameterHash { get; private set; }
    public int AttackCombo3ParameterHash { get; private set; }
    public int DeadParameterHash { get; private set; }
    public int KnockBackParameterHash { get; private set; }
    public int FallParameterHash { get; private set; }
    public int LandingParameterHash { get; private set; }

    public PlayerAnimationData()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        AttackCombo1ParameterHash = Animator.StringToHash(attackCombo1ParameterName);
        AttackCombo2ParameterHash = Animator.StringToHash(attackCombo2ParameterName);
        AttackCombo3ParameterHash = Animator.StringToHash(attackCombo3ParameterName);
        DeadParameterHash = Animator.StringToHash(deadParameterName);
        KnockBackParameterHash = Animator.StringToHash(knockBackParameterName);
        FallParameterHash = Animator.StringToHash(fallParameterName);
        LandingParameterHash = Animator.StringToHash(landingParameterName);
    }
}
