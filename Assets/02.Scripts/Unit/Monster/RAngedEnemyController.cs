using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("원거리 적 설정")]
    [SerializeField] private Transform projectileOffset;


    /// <summary>
    /// 원거리 적의 공격 행동
    /// 투사체 생성 및 발사
    /// </summary>
    public override void AttackAction()
    {
        Collider[] hitColliders = GetTargetColliders(LayerMask.GetMask("Player"));

        foreach(var hitCollider in hitColliders)
        {
            // 투사체 생성 및 발사
            FireProjectile(hitCollider.transform.position);
        }
    }

    private void FireProjectile(Vector3 targetPos)
    {
        // 타겟까지의 방향 계산
        Vector3 direction = (targetPos + Vector3.up * 1.5f)- projectileOffset.position;
        direction.y = 0; // 수평 방향으로만 발사
        direction.Normalize();

        // 투사체를 풀에서 가져오기
        GameObject projectile = PoolManager.Instance.GetObject(PoolType.projectile);
        projectile.transform.position = projectileOffset.position;
        projectile.GetComponent<Projectile>()?.InitProjectile(direction, 10f, Condition.GetValue(ConditionType.AttackPower));
    }
}
