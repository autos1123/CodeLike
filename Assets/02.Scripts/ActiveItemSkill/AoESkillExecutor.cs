using UnityEngine;

public class AoESkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster)
    {
        var obj = PoolManager.Instance.GetObject(PoolType.AoE);
        obj.transform.position = caster.position + caster.forward * data.Range;
        obj.GetComponent<AoE>()?.Init(data); 
    }
}
