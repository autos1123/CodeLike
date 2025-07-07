using UnityEngine;

public class EnemyStateMachine : UnitStateMachine
{
    public EnemyController Enemy { get; }

    public float MovementSpeed { get; private set; }

    public GameObject Player { get; private set; }
    public Vector3 PatrolPoint { get; private set; }

    // State
    public EnemyIdleState IdleState { get; }
    public EnemyPatrolState PatrolState { get; }
    public EnemyChaseState ChaseState { get; }
    public EnemyAttackState AttackState { get; }

    public EnemyStateMachine(EnemyController enemy)
    {
        this.Enemy = enemy;
        Player = GameManager.Instance.Player;
        MovementSpeed = Enemy.Condition.GetValue(ConditionType.MoveSpeed);

        IdleState = new EnemyIdleState(this);
        PatrolState = new EnemyPatrolState(this);
        ChaseState = new EnemyChaseState(this);
        AttackState = new EnemyAttackState(this);

        Enemy.SetPatrolPivot();
        ChangeState(IdleState);
    }

    /// <summary>
    /// 타겟의 Transform 대입
    /// 실시간 위치 추적이 필요한 경우
    /// </summary>
    /// <param name="target"></param>
    public void SetPatrolPoint(Vector3 target)
    {
        PatrolPoint = target;
    }
}
