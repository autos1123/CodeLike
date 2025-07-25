using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        Debug.Log("targetPoint: " + targetPoint);
        var vfx = Object.Instantiate(
            Resources.Load<ParticleSystem>(data.VFX),
            targetPoint,
            Quaternion.identity
        );
        vfx.Play();

        var obj = PoolManager.Instance.GetObject(PoolType.AoE);
        obj.GetComponent<AoE>().Init(data);
        obj.transform.position = targetPoint;

        PoolManager.Instance.ReturnObject(obj.GetComponent<IPoolObject>(), 5);
    }

}
