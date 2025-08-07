using System.Collections;
using UnityEngine;

public class Zone:MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 3;
    [SerializeField] private BoxCollider col;

    private float damage;
    private float duration;
    private float timer;
    private float tickInterval = 1f;
    private float tickTimer;

    private ParticleSystem vfx; // ★ VFX 관리 변수

    public GameObject GameObject => gameObject;
    public PoolType PoolType => poolType;
    public int PoolSize => poolSize;

    public void Init(ActiveItemEffectData data)
    {
        // 1. 기존 VFX 정리
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

        // 2. 새로운 VFX 생성 및 자식으로 부착
        if(!string.IsNullOrEmpty(data.VFX))
        {
            var vfxPrefab = Resources.Load<ParticleSystem>(data.VFX);
            if(vfxPrefab != null)
            {
                vfx = Instantiate(vfxPrefab, transform);
                vfx.transform.localPosition = Vector3.zero;
                vfx.Play();
            }
        }

        // 3. 데이터 초기화
        damage = data.Power;
        duration = data.Duration > 0 ? data.Duration : 5f;
        timer = 0;
        tickTimer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        tickTimer += Time.deltaTime;
        if(tickTimer >= tickInterval)
        {
            tickTimer = 0;
            DoZoneDamage(col.size * 0.5f);
        }
        if(timer >= duration)
            PoolManager.Instance.ReturnObject(this);
    }

    private void DoZoneDamage(Vector3 halfExtents)
    {
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
