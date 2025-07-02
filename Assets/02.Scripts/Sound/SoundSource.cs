using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour ,IPoolObject
{
    public SoundSource soundSource;
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize;
    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    private void Start()
    {
        soundSource = GetComponent<SoundSource>();
    }

    public void returnPool()
    {
        PoolManager.Instance.ReturnObject(this);
    }
}
