using UnityEngine;

public class ExplosionEnemyController:EnemyController
{
    [Header("폭발 적 설정")]
    [SerializeField] private float stayTime = 1.0f; // 플레이어가 머무르는 시간
    private float enterTime = 0f; // 플레이어가 공격 범위에 들어온 시간

    protected override void Update()
    {
        base.Update();

        if(!isInitialized)
            return;

        if(!isPlaying)
            return;

        // 추적 상태에서
        if(StateMachine.CurrentStateType == EnemyStateType.Chase)
        {
            // 공격 범위에 플레이어가 들어오면
            if(IsInRange(ConditionType.AttackRange))
            {
                // 플레이어가 머무른 시간을 체크
                if(enterTime == 0) enterTime = Time.time;

                if(Time.time - enterTime >= stayTime)
                {
                    // 일정 시간 이상 머무르면 공격 행동 수행
                    AttackAction();
                }
            }
            else 
            {
                // 누적 시간 초기화
                enterTime = 0f;
            }
        }
    }

    public override void AttackAction()
    {
        // 플레이어에게 데미지 입힌 후
        Player.GetComponent<IDamagable>().GetDamaged(100);

        // 본인은 자폭
        _CombatController.GetDamaged(Condition.GetTotalMaxValue(ConditionType.HP) + 1);
    }

    protected override void SetEnemyState()
    {
        StateMachine.AddState(EnemyStateType.Idle, new EnemyIdleState(StateMachine));
        StateMachine.AddState(EnemyStateType.Chase, new EnemyChaseState(StateMachine));
        StateMachine.AddState(EnemyStateType.Die, new EnemyDieState(StateMachine));
        StateMachine.AddState(EnemyStateType.Hit, new EnemyHitState(StateMachine));

        StateMachine.StartStateMachine(EnemyStateType.Idle);
    }

    public override bool IsInRange(ConditionType rangeType)
    {
        if(Player.GetComponent<PlayerController>().Condition.IsDied)
            return false; // 플레이어가 죽은 경우 추적하지 않음

        Vector3 targetPos = Player.transform.position;
        Vector3 curPos = transform.position;

        // 현재 뷰 모드가 2D인 경우 x축과 y축만 고려하여 거리 계산
        // 또는 추적을 위한 탐색인 경우
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D || rangeType == ConditionType.ChaseRange)
        {
            targetPos.z = 0;
            curPos.z = 0;
        }

        float playerDistanceSqr = (targetPos - curPos).sqrMagnitude;
        float range = Condition.GetTotalCurrentValue(rangeType);

        return playerDistanceSqr <= range * range;
    }
}
