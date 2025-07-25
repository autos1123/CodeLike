using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoE:MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private ActiveItemEffectData activeItemEffectData;
    private float timer;
    private LayerMask targetMask;
    public float tickInterval = 1f;
    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    public void Init(ActiveItemEffectData data)
    {
        activeItemEffectData = data;
        timer = data.Duration;
    }

    private void OnEnable()
    {
        timer = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if(((1 << other.gameObject.layer) & LayerMask.GetMask(LayerName.Enemy)) != 0)
        {
            timer += Time.deltaTime;
            if(timer >= tickInterval)
            {
                timer = 0f;
                other.GetComponent<IDamagable>()?.GetDamaged(activeItemEffectData.Power);
            }
        }
    }
}
