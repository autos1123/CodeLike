using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public enum PoolLoad
{
    None,
    Pool,
    Pool1,
    Pool2,
}

[DefaultExecutionOrder(-100)]
public class PoolManager:MonoSingleton<PoolManager>
{
    [SerializeField] List<GameObject> poolObjectList = new List<GameObject>();
    private List<IPoolObject> pools = new List<IPoolObject>();
    private Dictionary<PoolType, Queue<GameObject>> poolObjects = new Dictionary<PoolType, Queue<GameObject>>();
    private Dictionary<PoolType, GameObject> registeredObj = new Dictionary<PoolType, GameObject>();
    private Dictionary<PoolType, Transform> parentCache = new Dictionary<PoolType, Transform>();

    public bool IsInitialized { get; private set; } = false;
    protected override void Awake()
    {
        LoadPoolsAsync(PoolLoad.Pool);
    }

    /// <summary>
    /// 라벨을 통해 어드레서블에 올린 데이터 탐색하여 저장
    /// </summary>
    private void LoadPoolsAsync(PoolLoad poolLoad)
    {
        Addressables.LoadAssetsAsync<GameObject>(
            poolLoad.ToString(),
            (GameObject) =>
            {
                poolObjectList.Add(GameObject);
            }
        ).Completed += (handle) =>
        {
            foreach(var obj in poolObjectList)
            {
                if(obj.TryGetComponent<IPoolObject>(out var ipool))
                {
                    pools.Add(ipool);
                }
                else
                {
                    Debug.LogError($"오브젝트에 IPoolObject이 상속 되어 있지 않습니다. {obj.name}");
                }
            }
            foreach(var pool in pools)
            {
                CreatePool(pool, pool.PoolSize);
            }
            IsInitialized = true;
        };
    }

    private void CreatePool(IPoolObject iPoolObject, int poolsize)
    {
        if(poolObjects.ContainsKey(iPoolObject.PoolType))
        {
            Debug.LogWarning($"등록된 풀이 있습니다. : {iPoolObject.PoolType}");
            return;
        }

        string poolName = iPoolObject.PoolType.ToString();
        PoolType poolType = iPoolObject.PoolType;
        GameObject poolObject = iPoolObject.GameObject;

        Queue<GameObject> newPool = new Queue<GameObject>();
        GameObject prentObj = new GameObject(poolName) { transform = { parent = transform } };
        parentCache[poolType] = prentObj.transform;

        for(int i = 0; i < poolsize; i++)
        {

            GameObject obj = Instantiate(poolObject, prentObj.transform);
            obj.name = poolName;
            obj.SetActive(false);
            newPool.Enqueue(obj);
        }

        poolObjects[poolType] = newPool;
        registeredObj[poolType] = poolObject;
    }

    public GameObject GetObject(PoolType poolType)
    {
        string poolName = poolType.ToString();
        if(!poolObjects.TryGetValue(poolType, out Queue<GameObject> pool))
        {
            Debug.LogWarning($"등록된 풀이 없습니다. : {poolType}");
            return null;
        }

        if(pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject prefab = registeredObj[poolType];
            GameObject newObj = Instantiate(prefab);
            newObj.name = poolName;
            newObj.transform.SetParent(parentCache[poolType]);
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnObject(IPoolObject obj, float returnTime = 0, UnityAction action = null)
    {
            StartCoroutine(DelayedReturnObject(obj, returnTime, action));
    }

    IEnumerator DelayedReturnObject(IPoolObject obj, float returnTime, UnityAction action)
    {
        if(!poolObjects.ContainsKey(obj.PoolType))
        {
            Debug.LogWarning($"등록된 풀이 없습니다. : {obj.PoolType}");
            CreatePool(obj, 1);
        }

        yield return new WaitForSeconds(returnTime);
        ReturnObject(obj, action);
    }

    public void ReturnObject(IPoolObject obj, UnityAction action)
    {
        if(obj == null || obj.GameObject == null) return;

        obj.GameObject.SetActive(false);
        obj.GameObject.transform.position = Vector3.zero;
        action?.Invoke();
        poolObjects[obj.PoolType].Enqueue(obj.GameObject);
        obj.GameObject.transform.SetParent(parentCache[obj.PoolType]);
    }
    public void RemovePool(PoolType poolType)
    {
        Destroy(parentCache[poolType].gameObject);
        parentCache.Remove(poolType);
        poolObjects.Remove(poolType);
        registeredObj.Remove(poolType);
    }
}
