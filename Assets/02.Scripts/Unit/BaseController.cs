using System.Collections;
using UnityEngine;

public class BaseController:MonoBehaviour, IDamagable
{
    protected bool isInitialized = false; // ConditionData 초기화 여부 플래그

    [Header("ConditionData SO (엑셀 기반 SO 연결)")]
    [SerializeField] protected ConditionData data;
    private BaseCondition baseCondition;
    [SerializeField] protected int ID;

    public Rigidbody _Rigidbody { get; protected set; }
    public Animator _Animator { get; protected set; }
    public ConditionData Data => data;

    // 캐릭터 정지 관련
    protected bool isPlaying = true;
    protected Vector3 velocityTmp;

    protected virtual void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _Animator = GetComponentInChildren<Animator>();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(WaitForDataLoad());
    }

    protected virtual void OnDisable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.onGameStateChange -= OnGameStateChange;
    }

    protected virtual void Start()
    {
    }

    public virtual bool GetDamaged(float damage)
    {
        return true;
    }

    protected virtual void Initialize()
    {
        data = TableManager.Instance.GetTable<ConditionDataTable>().GetDataByID(ID);
        data.InitConditionDictionary();
        GameManager.Instance.onGameStateChange += OnGameStateChange;
    }

    protected virtual IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => TableManager.Instance.loadComplete && GameManager.HasInstance);
        Initialize();
    }

    /// <summary>
    /// 공격 범위 내에 있는 타겟 콜라이더를 반환합니다.
    /// </summary>
    /// <param name="layer">타겟 레이어</param>
    /// <returns></returns>
    public virtual Collider[] GetTargetColliders(LayerMask layer)
    {
        Collider[] hitColliders;

        if(!data.TryGetCondition(ConditionType.AttackRange, out float attackRange))
        {
            Debug.LogWarning("Attack range not set for MeleeEnemyController.");
            attackRange = 0.0f; // 기본값 설정
        }

        // 공격 범위의 중심점 설정
        // 중심점 위치는 오브젝트의 전방 방향으로 설정
        Vector3 pivot = transform.position + (transform.forward * (attackRange / 2.0f)) + Vector3.up;

        // 2D와 3D 모드에 따라 다른 오버랩 박스 크기 사용
        // 2D 모드에서는 z축을 늘려 x, y축만 고려할 수 있도록 
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            hitColliders = Physics.OverlapBox(pivot, new Vector3(attackRange, 1.5f, 100), Quaternion.identity, layer);
            return hitColliders;
        }

        hitColliders = Physics.OverlapBox(pivot, new Vector3(attackRange, 1.5f, attackRange), Quaternion.identity, layer);
        return hitColliders;
    }

    protected virtual void OnDrawGizmos()
    {
        if(Application.isPlaying && isInitialized)
        {
            if(data.TryGetCondition(ConditionType.AttackRange, out float attackRange))
            {
                // 공격 범위를 시각적으로 표시
                Gizmos.color = Color.red;
                Vector3 pivot = transform.position + (transform.forward * (attackRange / 2.0f)) + Vector3.up;
                Gizmos.DrawWireCube(pivot, new Vector3(attackRange, 1.5f, ViewManager.Instance.CurrentViewMode == ViewModeType.View2D ? 100 : attackRange));
            }
        }
    }

    protected void OnGameStateChange()
    {
        if(GameManager.Instance.curGameState == GameState.Play)
        {
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
        }

        SetCharacterPauseMode(isPlaying);
    }

    protected virtual void SetCharacterPauseMode(bool isPlaying)
    {
        if(!isPlaying)
        {
            velocityTmp = _Rigidbody.velocity; // 현재 속도를 저장
            _Rigidbody.velocity = Vector3.zero; // 게임이 일시정지되면 Rigidbody 속도 초기화
            _Rigidbody.useGravity = false; // 중력 비활성화

            _Animator.speed = 0;
        }
        else
        {
            _Rigidbody.velocity = velocityTmp; // 게임이 일시정지되면 Rigidbody 속도 초기화
            _Rigidbody.useGravity = true; // 중력 비활성화

            _Animator.speed = 1;
        }
    }
}
