using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        var go = PoolManager.Instance.GetObject(PoolType.projectile); //게임오브젝트 생성
        go.transform.position = caster.position;
        Vector3 dir = (targetPoint).normalized;

        go.GetComponent<Projectile>()?.InitProjectile(dir, 10, data.Power, data.Range);
    }
}
