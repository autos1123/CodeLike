using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    /// <summary>
    /// 근거리 적의 공격 행동
    /// 적 충돌 체크 및 공격
    /// </summary>
    public override void AttackAction()
    {
        base.AttackAction();

        if(!data.TryGetCondition(ConditionType.AttackRange, out float attackRange))
        {
            Debug.LogWarning("Attack range not set for MeleeEnemyController.");
            attackRange = 0.0f; // 기본값 설정
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Player"));
        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable player))
            {
                if(!data.TryGetCondition(ConditionType.AttackPower, out float power))
                {
                    power = 0.0f;
                }

                // 플레이어에게 피해를 입히는 로직
                player.GetDamaged(power);
            }
        }
    }
}
