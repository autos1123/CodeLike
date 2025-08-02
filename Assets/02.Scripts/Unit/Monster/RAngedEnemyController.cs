using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("원거리 적 설정")]
    [SerializeField] private Transform projectileOffset;
    [Header("투사체 방식")]
    [SerializeField] private int projectileType;


    /// <summary>
    /// 원거리 적의 공격 행동
    /// 투사체 생성 및 발사
    /// </summary>
    public override void AttackAction()
    {
        Collider[] hitColliders = _CombatController.GetTargetColliders(LayerMask.GetMask("Player"));

        foreach(var hitCollider in hitColliders)
        {
            // 투사체 생성 및 발사
            FireProjectile(hitCollider.transform.position);
        }
    }

    private void FireProjectile(Vector3 targetPos)
    {
        // 타겟까지의 방향 계산
        Vector3 direction = (targetPos + Vector3.up * 1.5f) - projectileOffset.position;
        direction.y = 0; // 수평 방향으로만 발사
        direction.Normalize();
        PoolType poolToUse = projectileType switch
        {
            0 => PoolType.projectile,       // 기존 TestProjectile
            1 => PoolType.ArrowProjectile,  // 새로 추가한 화살
            _ => PoolType.projectile
        };

        GameObject projectile = PoolManager.Instance.GetObject(poolToUse);
        projectile.transform.position = projectileOffset.position;

        projectile.GetComponent<Projectile>()?.InitProjectile(
            direction,
            10f,
            Condition.GetTotalCurrentValue(ConditionType.AttackPower)
        );
    }

    protected override void SetEnemyState()
    {
        StateMachine.AddState(EnemyStateType.Idle, new EnemyIdleState(StateMachine));
        StateMachine.AddState(EnemyStateType.Patrol, new EnemyPatrolState(StateMachine));
        StateMachine.AddState(EnemyStateType.Chase, new EnemyChaseState(StateMachine));
        StateMachine.AddState(EnemyStateType.Attack, new EnemyAttackState(StateMachine));
        StateMachine.AddState(EnemyStateType.Die, new EnemyDieState(StateMachine));
        StateMachine.AddState(EnemyStateType.Hit, new EnemyHitState(StateMachine));

        StateMachine.StartStateMachine(EnemyStateType.Idle);
    }

    public override AnimationClip GetPatternAnimationClip()
    {
        throw new System.NotImplementedException();
    }
}
