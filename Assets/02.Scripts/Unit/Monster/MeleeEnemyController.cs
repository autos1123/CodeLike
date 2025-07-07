using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    /// <summary>
    /// 근거리 적의 공격 행동
    /// 적 충돌 체크 및 공격
    /// </summary>
    public override void AttackAction()
    {
        Collider[] hitColliders = GetTargetColliders(LayerMask.GetMask("Player"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable player))
            {
                // 플레이어에게 피해를 입히는 로직
                if(!player.GetDamaged(Condition.GetValue(ConditionType.AttackPower)))
                {
                    StateMachine.ChangeState(StateMachine.IdleState);
                }
            }
        }
    }
}
