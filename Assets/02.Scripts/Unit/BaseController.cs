using System.Collections;
using UnityEngine;

public abstract class BaseController<T>:MonoBehaviour, IDamagable where T : BaseCondition
{
    protected bool isInitialized = false; // ConditionData 초기화 여부 플래그

    [Header("ConditionData SO (엑셀 기반 SO 연결)")]
    [SerializeField] protected int ID;

    public T Condition { get; protected set; } // 제네릭 타입으로 ConditionData 접근 
    public Rigidbody _Rigidbody { get; protected set; }
    public Animator _Animator { get; protected set; }

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
        Knockback();

        float defense = Condition.GetValue(ConditionType.Defense);
        float reducedDamage = Mathf.Max(0, damage - defense);

        if(Condition.GetDamaged(reducedDamage))
        {
            Die();
            return false;
        }

        return true;
    }

    protected abstract void Die();

    private void Knockback()
    {
        Vector3 dir = -transform.forward;  // 항상 내 뒤쪽
        float knockbackStrength = 7f;      // 고정 세기

        _Rigidbody.AddForce(dir * knockbackStrength, ForceMode.Impulse);
    }

    protected virtual void Initialize()
    {
        GameManager.Instance.onGameStateChange += OnGameStateChange;
    }

    /// <summary>
    /// ConditionData를 초기화합니다.
    /// BaseCondition을 상속받는 객체의 생성자에 전달
    /// </summary>
    /// <returns></returns>
    protected ConditionData InitConditionData()
    {
        ConditionData data = TableManager.Instance.GetTable<ConditionDataTable>().GetDataByID(ID);
        data.InitConditionDictionary();

        return data;
    }

    protected virtual IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => TableManager.Instance.loadComplete && GameManager.HasInstance && ViewManager.HasInstance);
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

        float attackRange = Condition.GetValue(ConditionType.AttackRange);

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

    /// <summary>
    /// 게임 상태가 변경될 때 호출되는 메서드입니다.
    /// </summary>
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

    /// <summary>
    /// 게임 상태에 따라 캐릭터의 일시정지 모드를 설정합니다.
    /// </summary>
    /// <param name="isPlaying"></param>
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

    protected virtual void OnDrawGizmos()
    {
        if(Application.isPlaying && isInitialized)
        {
            float attackRange = Condition.GetValue(ConditionType.AttackRange);

            // 공격 범위를 시각적으로 표시
            Gizmos.color = Color.red;
            Vector3 pivot = transform.position + (transform.forward * (attackRange / 2.0f)) + Vector3.up;
            Gizmos.DrawWireCube(pivot, new Vector3(attackRange, 1.5f, ViewManager.Instance.CurrentViewMode == ViewModeType.View2D ? 100 : attackRange));
        }
    }
}
