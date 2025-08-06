using UnityEngine;

public class ProjectileSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster)
    {
        var obj = PoolManager.Instance.GetObject(PoolType.PlayerProjectile);
        obj.transform.position = caster.position;
        Vector3 dir = caster.forward;
        // VFX 경로를 Init에 전달!
        obj.GetComponent<Projectile>()?.InitProjectile(
            dir,
            data.Range,       // speed
            data.Power,       // damage
            data.Duration,    // lifeTime
            1,                // hitCount (관통: 필요에 따라 조절)
            data.VFX          // ★ VFX 경로
        );
    }
}
