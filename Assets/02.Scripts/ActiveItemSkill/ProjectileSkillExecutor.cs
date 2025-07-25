using UnityEngine;

public class ProjectileSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        var go = PoolManager.Instance.GetObject(PoolType.PlayerProjectile);
        go.transform.position = caster.position;

        Vector3 dir = (targetPoint - caster.position).normalized;
        go.GetComponent<Projectile>()?.InitProjectile(dir, 10, data.Power, data.Range);
    }
}
