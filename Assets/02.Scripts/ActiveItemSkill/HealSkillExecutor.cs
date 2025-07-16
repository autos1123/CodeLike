using UnityEngine;

public class HealSkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        if(caster.TryGetComponent<BaseCondition>(out var baseCondition))
        {
            baseCondition.GetDamaged(data.Power);
        }
    }
}
