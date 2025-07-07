using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkillExecutor : ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector2 targetPoint)
    {
        Debug.Log(caster.transform.position);

        var go = PoolManager.Instance.GetObject(PoolType.projectile); //게임오브젝트 생성
        go.transform.position = caster.position;
        Vector3 dir = (targetPoint).normalized;

       go.GetComponent<Projectile>()?.init(data.Power, 1 , null);
       go.GetComponent<Projectile>()?.Launch(dir, data.Range);
    }
}
