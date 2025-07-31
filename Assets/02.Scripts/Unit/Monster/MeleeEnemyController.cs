using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    /// <summary>
    /// 근거리 적의 공격 행동
    /// 적 충돌 체크 및 공격
    /// </summary>
    public override void AttackAction()
    {
        Collider[] hitColliders = _CombatController.GetTargetColliders(LayerMask.GetMask("Player"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable player))
            {
                // 플레이어에게 피해를 입히는 로직
                if(!player.GetDamaged(Condition.GetTotalCurrentValue(ConditionType.AttackPower)))
                {
                    StateMachine.ChangeState(EnemyStateType.Idle);
                }
            }
        }
    }
    
    public override AnimationClip GetPatternAnimationClip()
    {
        throw new System.NotImplementedException();
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
}
