using System.Collections;
using UnityEngine;

public class ZoneSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster)
    {
        var obj = PoolManager.Instance.GetObject(PoolType.Zone);
        obj.transform.position = caster.position + caster.forward * data.Range;
        obj.GetComponent<Zone>()?.Init(data);

    }
}