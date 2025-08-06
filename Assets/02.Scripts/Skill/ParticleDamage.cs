using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDamage:MonoBehaviour
{
    public float Damage;
    public SkillType skillType;

    public void Init(float damage, SkillType type)
    {
        Damage = damage;
        skillType = type;
    }

    private void OnParticleCollision(GameObject other)
    {
        if(!other.TryGetComponent<IDamagable>(out var damagable))
            return;

        switch(skillType)
        {
            case SkillType.AoE:
                // 1회만 강하게 (예: Damage)
                damagable.GetDamaged(Damage);
                break;

            case SkillType.Zone:
                // 지속딜: 프레임마다 들어감 (예: Damage * Time.deltaTime)
                damagable.GetDamaged(Damage * Time.deltaTime);
                break;

            case SkillType.Projectile:
                // 단일 타겟, 1회
                damagable.GetDamaged(Damage);
                break;
        }
    }
}

