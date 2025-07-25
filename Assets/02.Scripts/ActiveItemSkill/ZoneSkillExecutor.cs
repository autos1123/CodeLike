using System.Collections;
using UnityEngine;

public class ZoneSkillExecutor:ISkillExecutor
{
    public void Execute(ActiveItemEffectData data, Transform caster, Vector3 targetPoint)
    {
        Debug.Log("targetPoint: " + targetPoint);

        // 1. VFX를 PoolManager에서 꺼내서 사용
        var vfxObj = PoolManager.Instance.GetObject(PoolType.VFX);
        if(vfxObj != null)
        {
            vfxObj.transform.position = targetPoint;
            vfxObj.transform.rotation = Quaternion.identity;

            var ps = vfxObj.GetComponent<ParticleSystem>();
            if(ps != null)
            {
                ps.Play();
                // 자동 반환: duration 후에 풀로 복귀
                vfxObj.GetComponent<PooledVFX>()?.PlayVFX(ps.main.duration);
            }
        }
        else
        {
            Debug.LogWarning("VFX 풀에 오브젝트가 없습니다.");
        }

        // 2. Zone 오브젝트(AoE)도 풀에서 꺼내서 사용
        //var obj = PoolManager.Instance.GetObject(PoolType.AoE);
        //if(obj != null)
        //{
        //    obj.GetComponent<AoE>().Init(data);
        //    obj.transform.position = targetPoint;
        //    PoolManager.Instance.ReturnObject(obj.GetComponent<IPoolObject>(), 5);
        //}
        //else
        //{
        //    Debug.LogWarning("AoE 풀에 오브젝트가 없습니다.");
        //}
    }
}
