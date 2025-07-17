using UnityEngine;

public class AoESkillExecutor :ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        var pos = caster.transform.position;
        /*        var vfx = Object.Instantiate(
                    Resources.Load<ParticleSystem>(data.VFX),
                    targetPoint,
                    Quaternion.identity
                );
                vfx.Play();*/

        var obj = PoolManager.Instance.GetObject(PoolType.AoE);
        obj.GetComponent<AoE>().Init(data);
        obj.transform.position = caster.position;

        PoolManager.Instance.ReturnObject(obj.GetComponent<IPoolObject>(), 1);
    }
}
