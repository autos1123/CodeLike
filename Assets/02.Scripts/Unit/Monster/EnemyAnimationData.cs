using UnityEngine;

public class EnemyAnimationData
{
    private string idleParameterName = "Idle";
    private string patrolParameterName = "Patrol";
    private string chaseParameterName = "Chase";
    private string attackParameterName = "Attack";

    public int IdleParameterHash { get; private set; }
    public int PatrolParameterHash { get; private set; }
    public int ChaseParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        PatrolParameterHash = Animator.StringToHash(patrolParameterName);
        ChaseParameterHash = Animator.StringToHash(chaseParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
    }
}
