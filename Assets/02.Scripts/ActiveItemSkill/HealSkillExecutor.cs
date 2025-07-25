using UnityEngine;

public class HealSkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        if(caster.TryGetComponent<IDamagable>(out var damagable))
        {
            damagable.GetDamaged(-data.Power);
            var vfx = Object.Instantiate(
                    Resources.Load<ParticleSystem>(data.VFX),
                    targetPoint,
                    Quaternion.identity
                );
            vfx.Play();
        }
    }
}
