using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("원거리 공격 관련 변수")]
    [SerializeField] private GameObject projectilePrefab; // 투사체 프리팹

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

    }
}
