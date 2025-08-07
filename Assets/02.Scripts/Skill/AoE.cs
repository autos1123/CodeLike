using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoE:MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 5;
    [SerializeField] private BoxCollider col;

    private float damage;
    private ParticleSystem vfx; // 현재 붙은 VFX 참조

    public GameObject GameObject => gameObject;
    public PoolType PoolType => poolType;
    public int PoolSize => poolSize;

    public void Init(ActiveItemEffectData data)
    {
        // 기존 VFX 제거
        if(vfx != null)
        {
            Destroy(vfx.gameObject);
            vfx = null;
        }
        foreach(Transform child in transform)
        {
            if(child.GetComponent<ParticleSystem>() != null)
                Destroy(child.gameObject);
        }

        // 새로운 VFX 생성 및 자식으로 부착
        if(!string.IsNullOrEmpty(data.VFX))
        {
            var vfxPrefabObj = Resources.Load<GameObject>(data.VFX);
            if(vfxPrefabObj != null)
            {
                var vfxObj = Instantiate(vfxPrefabObj, transform);
                vfxObj.transform.localPosition = Vector3.zero;
                vfx = vfxObj.GetComponent<ParticleSystem>();
                if(vfx != null) vfx.Play();
            }
            else
            {
                Debug.LogError("VFX 프리팹을 찾을 수 없습니다: " + data.VFX);
            }
        }

        damage = data.Power;
        DoAoEDamage(data.Range);

        PoolManager.Instance.ReturnObject(this, 0.5f); // 0.5초 후 반환 (VFX 효과 등 고려)
    }

    private void DoAoEDamage(float range)
    {
        Vector3 halfExtents = Vector3.one * range;
        Collider[] hits = Physics.OverlapBox(
            transform.position,
            halfExtents,
            transform.rotation,
            LayerMask.GetMask("Enemy")
        );
        foreach(var col in hits)
        {
            if(col.TryGetComponent<IDamagable>(out IDamagable target))
            {
                target.GetDamaged(damage);
                PoolingDamageUI damageUI = PoolManager.Instance.GetObject(PoolType.DamageUI).GetComponent<PoolingDamageUI>();
                damageUI.InitDamageText(target.GetDamagedPos(), DamageType.Normal, damage);
            }
        }
    }

    private void OnDisable()
    {
        if(vfx != null)
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
