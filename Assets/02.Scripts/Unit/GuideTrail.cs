using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTrail : MonoBehaviour, IPoolObject
{
    [Header("Pooling Setting")]
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize = 10;

    [Header("Trail Setting")]
    [SerializeField] private float speed;
    [SerializeField] private float arriveDistance;

    private Vector3 target;
    private TrailRenderer trailRenderer;

    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, target) < arriveDistance)
        {
            PoolManager.Instance.ReturnObject(this, null);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    public void Initialize(Vector3 start, Vector3 target)
    {
        this.target = target;
        transform.position = start;
    }
}
