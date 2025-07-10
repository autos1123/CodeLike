using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AoESkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        Debug.Log("범위 공격임");
        return;


        var pos = caster.transform.position;
        var vfx = Object.Instantiate(
            Resources.Load<ParticleSystem>(data.VFX),
            targetPoint,
            Quaternion.identity
        );
        vfx.Play();

        Collider[] hits = Physics.OverlapSphere(targetPoint, data.Range, LayerMask.GetMask(LayerName.Enemy));
        foreach(var c in hits)
        {
            c.GetComponent<BaseCondition>()?.GetDamaged(data.Power);
        }
    }
}
