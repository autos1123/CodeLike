using UnityEngine;

public class HealSkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        if(caster.TryGetComponent<PlayerController>(out var playerController))
        {
            //플레이어 컨트롤러에서 회복
        }
    }
}
