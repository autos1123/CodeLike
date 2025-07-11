using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController<T>:MonoBehaviour, IDamagable where T : BaseCondition
{
    protected bool isInitialized = false; // ConditionData 초기화 여부 플래그

    [Header("ConditionData SO (엑셀 기반 SO 연결)")]
    [SerializeField] protected int ID;

    public T Condition { get; protected set; } // 제네릭 타입으로 ConditionData 접근 
    public Rigidbody _Rigidbody { get; protected set; }
    public Animator _Animator { get; protected set; }
    protected BoxCollider col;

    // 캐릭터 충돌체 변환 관련
    protected Vector3 colliderSizeTmp;
    [SerializeField] private float fullSize;

    // 공격 범위 관련
    protected Vector3 forwardDir;
    protected float attackAngle = 100f; // 공격 각도

    // 캐릭터 정지 관련
    protected bool isPlaying = true;
    protected Vector3 velocityTmp;

    [Header("Visual Settings")]
    [SerializeField] protected Transform visualTransform;
    [SerializeField] protected float visualRotateSpeed = 10f;

    public Transform VisualTransform => visualTransform;
    public float VisualRotateSpeed => visualRotateSpeed;

    protected virtual void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _Animator = GetComponentInChildren<Animator>();
        col = GetComponent<BoxCollider>();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(WaitForDataLoad());
        ViewManager.Instance.OnViewChanged += OnViewChange;
    }

    protected virtual void OnDisable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.onGameStateChange -= OnGameStateChange;

        if(ViewManager.HasInstance)
            ViewManager.Instance.OnViewChanged -= OnViewChange;
    }

    protected virtual void Start()
    {
    }

    public virtual bool GetDamaged(float damage)
    {
        // Knockback();

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

    //private void Knockback()
    //{
    //    Vector3 dir = -transform.forward;  // 항상 내 뒤쪽
    //    float knockbackStrength = 7f;      // 고정 세기

    //    _Rigidbody.AddForce(dir * knockbackStrength, ForceMode.Impulse);
    //}

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



        // 2D와 3D 모드에 따라 다른 오버랩 박스 크기 사용
        // 2D 모드에서는 z축을 늘려 x, y축만 고려할 수 있도록 
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
        {
            // 공격 범위의 중심점 설정
            // 중심점 위치는 오브젝트의 전방 방향으로 설정
            Vector3 pivot = transform.position + (visualTransform.forward * (attackRange / 2.0f)) + Vector3.up;

            hitColliders = Physics.OverlapBox(pivot, new Vector3(attackRange, 1.5f, 100), Quaternion.identity, layer);
            return hitColliders;
        }

        hitColliders = Physics.OverlapSphere(transform.position, attackRange, layer);
        List<Collider> filteredColliders = new List<Collider>();

        foreach(Collider collider in hitColliders)
        {
            Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(visualTransform.forward, directionToTarget);

            if(angle <= attackAngle) // 원하는 각도
            {
                filteredColliders.Add(collider);
            }
        }
        return filteredColliders.ToArray();
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
            _Rigidbody.useGravity = true; // 중력 활성화

            _Animator.speed = 1;
        }
    }

    public virtual void OnViewChange(ViewModeType viewMode)
    {
        if(viewMode == ViewModeType.View2D)
        {
            _Rigidbody.detectCollisions = false;

            // 콜라이더 사이즈 확장
            colliderSizeTmp = col.size;
            col.center = new Vector3(transform.position.z, col.center.y, 0); // 2D 모드에서는 콜라이더 중심을 약간 위로 이동
            col.size = new Vector3(fullSize, colliderSizeTmp.y, colliderSizeTmp.z); // 2D 모드에서는 z축을 늘려서 공격 범위 확장

            // 겹치는 오브젝트가 있을 경우 콜라이더 위치 조정
            SetPositionWhenViewChanged();

            _Rigidbody.detectCollisions = true;
        }
        else if(viewMode == ViewModeType.View3D)
        {
            col.center = new Vector3(0, col.center.y, 0); // 2D 모드에서는 콜라이더 중심을 약간 위로 이동
            col.size = colliderSizeTmp; // 3D 모드에서는 원래 크기로 복원
        }
    }

    /// <summary>
    /// 2D 모드에서 뷰가 변경될 때 콜라이더의 위치를 조정합니다.
    /// </summary>
    /// <param name="col"></param>
    private void SetPositionWhenViewChanged()
    {
        // 현재 콜라이더의 경계 박스에서 충돌한 오브젝트들을 가져옴
        Collider[] hit = Physics.OverlapBox(col.bounds.center, col.bounds.extents, Quaternion.identity, LayerMask.GetMask("Obstacle"), QueryTriggerInteraction.Ignore);

        if(hit.Length == 0) return;

        // 충돌한 콜라이더의 최대 x좌표와 최소 x좌표를 계산
        float maxX = hit[0].bounds.max.x;
        float minX = hit[0].bounds.min.x;
        for(int i = 1; i < hit.Length; i++)
        {
            if(hit[i].bounds.max.x > maxX)
            {
                maxX = hit[i].bounds.max.x;
            }
            if(hit[i].bounds.min.x < minX)
            {
                minX = hit[i].bounds.min.x;
            }
        }

        // 충돌한 콜라이더(col)의 왼쪽 경계(min.x)로부터 현재 위치까지의 x 거리 계산(+여유값 0.5f 추가)
        float distance = transform.position.x - minX + 0.5f;
        Vector3 direction = Vector3.left;

        if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 적 캐릭터는 오른쪽으로 이동
            distance = maxX - transform.position.x + 0.5f;
            direction = Vector3.right; 
        }

        // 현재 오브젝트를 왼쪽으로 distance만큼 이동시켜, 콜라이더에서 완전히 벗어나게 함
        transform.position = transform.position + direction * distance;
    }

    protected virtual void OnDrawGizmos()
    {
        if(Application.isPlaying && isInitialized)
        {
            float attackRange = Condition.GetValue(ConditionType.AttackRange);

            // 공격 범위를 시각적으로 표시
            Gizmos.color = Color.red;
            if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            {
                Vector3 pivot = transform.position + (visualTransform.forward * (attackRange / 2.0f)) + Vector3.up;
                Gizmos.DrawWireCube(pivot, new Vector3(attackRange, 1.5f, ViewManager.Instance.CurrentViewMode == ViewModeType.View2D ? 100 : attackRange));
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, attackRange);

                Gizmos.color = Color.black;
                float angle1 = -attackAngle / 2f;
                float angle2 = attackAngle / 2f;

                Vector3 dir1 = Quaternion.Euler(0, angle1, 0) * visualTransform.forward;
                Vector3 dir2 = Quaternion.Euler(0, angle2, 0) * visualTransform.forward;

                Vector3 point1 = transform.position + dir1 * attackRange;
                Vector3 point2 = transform.position + dir2 * attackRange;

                Gizmos.DrawLine(transform.position, point1);
                Gizmos.DrawLine(transform.position, point2);
            }
        }
    }
}
