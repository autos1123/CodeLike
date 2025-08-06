using UnityEngine;

public class HealSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster)
    {
        // 예시: PlayerController가 BaseCondition을 가진 경우
        if(caster.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Condition.Heal(data.Power);
        }

        // 이하 VFX 처리 코드는 동일하게 유지
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
