using UnityEngine;

public class HealSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster)
    {
        if(caster.TryGetComponent<IDamagable>(out var damagable))
        {
            damagable.GetDamaged(-data.Power);
        }

        if(!string.IsNullOrEmpty(data.VFX))
        {
            var vfxPrefab = Resources.Load<ParticleSystem>(data.VFX);
            if(vfxPrefab != null)
            {
                foreach(Transform child in caster)
                {
                    if(child.GetComponent<ParticleSystem>() != null)
                        Object.Destroy(child.gameObject);
                }

                var vfx = Object.Instantiate(vfxPrefab, caster);
                vfx.transform.localPosition = Vector3.zero;
                vfx.Play();

                Object.Destroy(
                    vfx.gameObject,
                    vfx.main.duration + vfx.main.startLifetime.constantMax
                );
            }
        }
    }
}
