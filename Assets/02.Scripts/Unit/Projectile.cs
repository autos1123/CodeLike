using UnityEngine;

public class Projectile:MonoBehaviour, IPoolObject
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;

    private float lifeTime;
    private float damage;
    private float speed;
    private int hitCount;

    private float ignoreTime = 0.1f;
    private float currentIgnore = 0f;

    private ParticleSystem vfx;

    // === 크리티컬 관련 필드 추가 ===
    private bool isCritical = false;
    private float criticalDamageMultiplier = 1f;

    public GameObject GameObject => gameObject;
    public PoolType PoolType => poolType;
    public int PoolSize => poolSize;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            PoolManager.Instance.ReturnObject(this);
            return;
        }

        if(currentIgnore > 0f)
            currentIgnore -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentIgnore > 0f) return;

        if(other.TryGetComponent(out IDamagable target) && ((1 << other.gameObject.layer) & layer) != 0)
        {
            // === 크리티컬 적용 ===
            float finalDamage = isCritical ? damage * criticalDamageMultiplier : damage;
            if(isCritical)
                Debug.Log("Projectile Critical Hit!");

            target.GetDamaged(finalDamage);
            hitCount--;
            if(hitCount <= 0)
                PoolManager.Instance.ReturnObject(this);
        }
    }

    /// <summary>
    /// 발사체 초기화 (크리티컬 정보까지 확장)
    /// </summary>
    public void InitProjectile(
        Vector3 dir,
        float speed,
        float damage,
        float lifeTime = 3f,
        int hitCount = 1,
        string vfxPath = null,
        bool isCritical = false,
        float criticalDamageMultiplier = 1f)
    {
        transform.forward = dir.normalized;

        this.speed = speed;
        this.damage = damage;
        this.lifeTime = lifeTime > 0 ? lifeTime : 3f;
        this.hitCount = hitCount;

        this.isCritical = isCritical;
        this.criticalDamageMultiplier = criticalDamageMultiplier;

        currentIgnore = ignoreTime;

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

        if(!string.IsNullOrEmpty(vfxPath))
        {
            var vfxPrefab = Resources.Load<ParticleSystem>(vfxPath);
            if(vfxPrefab != null)
            {
                vfx = Instantiate(vfxPrefab, transform);
                vfx.transform.localPosition = Vector3.zero;
                vfx.Play();
            }
        }
    }

    private void OnDisable()
    {
        if(vfx != null)
            vfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
