using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolObject
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;

    private float lifeTime; // 투사체의 생명주기
    private float damage;
    private float speed;
    private int hitCount;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌 시 처리 로직
        if(other.TryGetComponent(out IDamagable target) && ((1 << other.gameObject.layer) & layer) != 0)
        {
            // 대상에게 피해를 입히는 로직
            target.GetDamaged(damage);

            hitCount--;

            // 투사체를 풀에 반환
            if(hitCount <= 0)
                PoolManager.Instance.ReturnObject(this);
        }
    }

    /// <summary>
    /// 투사체를 초기화합니다.
    /// </summary>
    /// <param name="dir">진행 방향</param>
    /// <param name="speed">진행 속도</param>
    /// <param name="damage">공격력</param>
    /// <param name="lifeTime">진행 거리</param>
    /// <param name="hitCount">관통 횟수, 기본 1</param>
    public void InitProjectile(Vector3 dir, float speed, float damage, float lifeTime = float.MaxValue, int hitCount = 1)
    {
        transform.forward = dir; // 방향 설정
        this.speed = speed;
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.hitCount = hitCount;
    }
}
