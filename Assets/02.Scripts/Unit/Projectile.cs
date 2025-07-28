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

    private float ignoreTime = 0.1f; // 발사 직후 무시할 시간
    private float currentIgnore = 0f;

    private ParticleSystem vfx; // 현재 붙은 VFX 참조

    // IPoolObject
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

        // 무시 시간 카운트
        if(currentIgnore > 0f)
            currentIgnore -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 발사 직후 ignoreTime 동안은 아무것도 처리하지 않음
        if(currentIgnore > 0f) return;

        // 타겟 레이어 체크
        if(other.TryGetComponent(out IDamagable target) && ((1 << other.gameObject.layer) & layer) != 0)
        {
            target.GetDamaged(damage);
            hitCount--;
            if(hitCount <= 0)
                PoolManager.Instance.ReturnObject(this);
        }
    }

    /// <summary>
    /// 발사체를 초기화(재사용)합니다.
    /// 반드시 외부에서 position, 방향(dir)을 명확히 넘겨주세요!
    /// </summary>
    public void InitProjectile(
        Vector3 dir,
        float speed,
        float damage,
        float lifeTime = 3f,
        int hitCount = 1,
        string vfxPath = null)
    {
        transform.forward = dir.normalized;

        this.speed = speed;
        this.damage = damage;
        this.lifeTime = lifeTime > 0 ? lifeTime : 3f;
        this.hitCount = hitCount;

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
