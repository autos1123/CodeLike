using UnityEngine;

public class Projectile : MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float speed = 10f;

    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    private void Update()
    {
        // 투사체의 이동 로직
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌 시 처리 로직
        if(other.TryGetComponent(out IDamagable target) && ((1 << other.gameObject.layer) & LayerMask.GetMask("Player")) != 0)
        {
            // 대상에게 피해를 입히는 로직
            target.GetDamaged(10f); // 예시로 10의 피해를 입힘
            // 투사체를 풀에 반환
            PoolManager.Instance.ReturnObject(this);
        }
    }
}
