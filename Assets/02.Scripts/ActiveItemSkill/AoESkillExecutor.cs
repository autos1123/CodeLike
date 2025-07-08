using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AoESkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        var pos = caster.transform.position;
        var vfx = Object.Instantiate(
            Resources.Load<ParticleSystem>(data.EffectPrefab),
            targetPoint,
            Quaternion.identity
        );
        vfx.Play();

        Collider2D[] hits = Physics2D.OverlapCircleAll(targetPoint, data.Range, LayerMask.GetMask(LayerName.Enemy));
        foreach(var c in hits)
        {
            c.GetComponent<BaseCondition>()?.GetDamaged(data.Power);
        }
    }
}
