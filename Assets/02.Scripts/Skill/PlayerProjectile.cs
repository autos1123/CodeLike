using UnityEngine;

public class PlayerProjectile:MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;

    private float damage;
    private float speed;
    private float lifeTime;
    private Vector3 direction;

    public GameObject GameObject => gameObject;
    public PoolType PoolType => poolType;
    public int PoolSize => poolSize;

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
            PoolManager.Instance.ReturnObject(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamagable>(out var damagable))
        {
            
            damagable.GetDamaged(damage);
            PoolManager.Instance.ReturnObject(this, null);
        }
    }

    public void Init(ActiveItemEffectData data, Vector3 dir)
    {
        damage = data.Power;
        speed = data.Range > 0 ? data.Range : 10f;
        lifeTime = data.Duration > 0 ? data.Duration : 3f;
        direction = dir.normalized;

        // (1) 파티클 생성 (데이터에서 경로 읽어옴)
        ParticleSystem particle = null;
        if(!string.IsNullOrEmpty(data.VFX))
        {
            var vfxPrefab = Resources.Load<ParticleSystem>(data.VFX);
            if(vfxPrefab != null)
            {
                particle = Instantiate(vfxPrefab, transform);
                particle.transform.localPosition = Vector3.zero;
                particle.Play();
            }
        }

        // (2) 파티클 크기 = Collider 크기 세팅
        if(particle != null)
        {
            // ★ 3D 파티클이면 startSizeX/Y/Z, 2D는 startSize
            var main = particle.main;
            float sizeX = main.startSizeX.constant * particle.transform.lossyScale.x;
            float sizeY = main.startSizeY.constant * particle.transform.lossyScale.y;
            float sizeZ = main.startSizeZ.constant * particle.transform.lossyScale.z;
        }
    }

}
