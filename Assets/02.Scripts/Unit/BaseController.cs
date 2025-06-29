using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour, IDamagable
{
    protected bool isInitialized = false; // ConditionData 초기화 여부 플래그

    [Header("ConditionData SO (엑셀 기반 SO 연결)")]
    [SerializeField] protected ConditionData data;
    [SerializeField] protected int ID;

    public Rigidbody _Rigidbody { get; protected set; }
    public ConditionData Data => data;

    protected virtual void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        StartCoroutine(WaitForDataLoad());
    }

    public bool GetDamaged(float damage)
    {
        throw new System.NotImplementedException();
    }

    protected virtual void Initialize()
    {
        data = GameManager.Instance.TableManager.GetTable<ConditionDataTable>().GetDataByID(ID);
        data.InitConditionDictionary();
    }

    protected IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => GameManager.Instance.TableManager.loadComplete);
        Initialize();
    }
}
