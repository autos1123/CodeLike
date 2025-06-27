using UnityEngine;
using UnityEngine.AI;

public class EnemyIdleState : EnemyBaseState
{
    private float waitingStartTime;
    private float waitingEndTime = 2f;
    private float patrolRadius = 5f; // 순찰 반경
    Vector3 nextPoint;

    public EnemyIdleState(EnemyStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void StateEnter()
    {
        moveSpeedModifier = 0; // Idle 상태에서는 이동 속도를 0으로 설정
        base.StateEnter();
        waitingStartTime = Time.time;
        nextPoint = GetWanderLocation();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(IsInChaseRange())
        {
            // MoveState로 변환
            stateMachine.ChangeState(stateMachine.ChaseState);
        }

        if(waitingEndTime <= Time.time - waitingStartTime)
        {
            // 대기 시간이 끝나면 Target을 다음 PatrolPoint로 설정 후 MoveState로 전환
            stateMachine.SetPatrolPoint(nextPoint);
            stateMachine.ChangeState(stateMachine.PatrolState);
        }
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        // 순찰 반경 내에서 랜덤한 위치를 탐색, 유효한 위치, 경로인지 확인
        // 탐색한 지점의 거리가 patrolRadius의 30% 이상인 경우에만 유효한 위치로 간주
        // 최대 30번 시도하여 유효한 위치를 찾습니다.
        for(int i = 0; i < 30; i++)
        {
            Vector2 samplePosV2 = Random.insideUnitCircle * patrolRadius;
            Vector3 sample = new Vector3(samplePosV2.x, stateMachine.Enemy.patrolPivot.y, samplePosV2.y);

            if(NavMesh.SamplePosition(stateMachine.Enemy.patrolPivot + sample, out hit, patrolRadius, NavMesh.AllAreas))
            {
                if(Vector3.Distance(stateMachine.Enemy.transform.position, hit.position) > patrolRadius * 0.3f)
                {
                    return hit.position; // 유효한 위치를 반환
                }
            }
        }

        return stateMachine.Enemy.patrolPivot; // Fallback to patrol pivot if no valid position found
    }
}
