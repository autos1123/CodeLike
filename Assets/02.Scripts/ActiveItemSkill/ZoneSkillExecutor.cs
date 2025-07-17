using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSkillExecutor :ISkillExecutor
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
        // 풀링으로 관리 예정 fx 구매후 fx 실행후 존스킬인경우에 몇초후 회수

        PoolManager.Instance.ReturnObject(obj.GetComponent<IPoolObject>(), 5);
    }
}
