using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationData
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string patrolParameterName = "Patrol";
    [SerializeField] private string chaseParameterName = "Chase";
    [SerializeField] private string attackParameterName = "Attack";

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
