using System.Collections;
using System.Collections.Generic;
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
        GetDamaged(Condition.GetValue(ConditionType.HP));
        StateMachine.ChangeState(EnemyStateType.Die);
    }

    protected override void SetEnemyState()
    {
        StateMachine.AddState(EnemyStateType.Idle, new EnemyIdleState(StateMachine));
        StateMachine.AddState(EnemyStateType.Chase, new EnemyChaseState(StateMachine));
        StateMachine.AddState(EnemyStateType.Die, new EnemyDieState(StateMachine));

        StateMachine.StartStateMachine(EnemyStateType.Idle);
    }

    public override bool IsInRange(ConditionType rangeType)
    {
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
        float range = Condition.GetValue(rangeType);

        return playerDistanceSqr <= range * range;
    }

    protected override void OnDrawGizmos()
    {
        if(Application.isPlaying && isInitialized)
        {
            float attackRange = Condition.GetValue(ConditionType.AttackRange);
            float chaseRange = Condition.GetValue(ConditionType.ChaseRange);
            Vector3 pivot;

            // 공격 범위를 시각적으로 표시
            Gizmos.color = Color.red;
            if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            {
                pivot = transform.position + (visualTransform.forward * (attackRange / 2.0f)) + Vector3.up;
                Gizmos.DrawWireCube(pivot, new Vector3(attackRange, 1.5f, ViewManager.Instance.CurrentViewMode == ViewModeType.View2D ? 100 : attackRange));
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, attackRange);

                Gizmos.color = Color.black;
                float angle1 = -attackAngle / 2f;
                float angle2 = attackAngle / 2f;

                Vector3 dir1 = Quaternion.Euler(0, angle1, 0) * visualTransform.forward;
                Vector3 dir2 = Quaternion.Euler(0, angle2, 0) * visualTransform.forward;

                Vector3 point1 = transform.position + dir1 * attackRange;
                Vector3 point2 = transform.position + dir2 * attackRange;

                Gizmos.DrawLine(transform.position, point1);
                Gizmos.DrawLine(transform.position, point2);
            }

            pivot = transform.position + (visualTransform.forward * (chaseRange / 2.0f)) + Vector3.up;
            // 적의 추적 범위를 시각적으로 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(pivot, new Vector3(chaseRange, 1.5f, 100));
        }
    }
}
