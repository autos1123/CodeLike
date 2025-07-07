using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float speed = 10f;

    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;


    private Rigidbody2D _rigidbody2D;

    float damage;
    int count;
    ParticleSystem vfxPrefab;

    public void init(float dmg, int count, ParticleSystem vfxPrefab)
    {
        this.damage = dmg;
        this.count = count;
        this.vfxPrefab = vfxPrefab;
    }
    public void Launch(Vector3 dir, float speed)
    {
        if(_rigidbody2D == null)
        {
            _rigidbody2D = transform.GetComponent<Rigidbody2D>();
        }

        _rigidbody2D.velocity = new Vector2(dir.x, dir.y) * speed;
        Debug.Log("발싸!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌 시 처리 로직
        if(other.TryGetComponent(out IDamagable target) && ((1 << other.gameObject.layer) & LayerMask.GetMask("Player")) != 0)
        {
            // 대상에게 피해를 입히는 로직
            target.GetDamaged(10f); // 예시로 10의 피해를 입힘
            // 투사체를 풀에 반환
            ReturnPool();
        }
    }

    public void ReturnPool()
    {
        PoolManager.Instance.ReturnObject(this);
    }

}
