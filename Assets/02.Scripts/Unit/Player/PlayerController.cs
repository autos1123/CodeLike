using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController:MonoBehaviour
{
    [Header("ConditionData SO (엑셀 기반 SO 연결)")]
    [SerializeField] private ConditionData conditionData;
    [SerializeField] private int ID;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayOffset = 0.1f;

    private PlayerInputHandler inputHandler;
    private Rigidbody rb;
    private Collider col;
    private bool isGrounded;

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerCondition condition { get; private set; }

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.freezeRotation = true;
        stateMachine = new PlayerStateMachine(this);
    }

    private void Start()
    {
        StartCoroutine(WaitForDataLoad());
    }

    private IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => GameManager.Instance.TableManager.loadComplete);

        conditionData = GameManager.Instance
            .TableManager
            .GetTable<ConditionDataTable>()
            .GetDataByID(ID);

        conditionData.InitConditionDictionary();
        condition = new PlayerCondition(conditionData);
        condition.Init(this, stateMachine);
    }

    private void Update()
    {
        // **카메라 전환 중엔 아무 동작 하지 않음**
        if(ViewManager.Instance.IsTransitioning)
            return;

        UpdateGrounded();

        if(inputHandler.JumpPressed && isGrounded)
            Jump();

        stateMachine.Update();
        inputHandler.ResetOneTimeInputs();
    }

    private void FixedUpdate()
    {
        // **카메라 전환 중엔 물리 업데이트도 스킵**
        if(ViewManager.Instance.IsTransitioning)
            return;

        stateMachine.PhysicsUpdate();
    }

    private void UpdateGrounded()
    {
        Vector3 origin = col.bounds.center;
        float distance = col.bounds.extents.y + groundRayOffset;

        bool hit = Physics.Raycast(
            origin,
            Vector3.down,
            distance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        if(hit != isGrounded)
            isGrounded = hit;

        Debug.DrawRay(origin, Vector3.down * distance, hit ? Color.green : Color.red);
    }

    public Vector3 Move(Vector2 input)
    {
        // MoveState 내부에서도 안전하게 무시 가능
        if(ViewManager.Instance.IsTransitioning)
            return Vector3.zero;

        float speed = condition.GetValue(ConditionType.MoveSpeed);

        Vector3 dir;
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            dir = new Vector3(input.x, 0f, 0f);
        else
        {
            var f = Camera.main.transform.forward; f.y = 0; f.Normalize();
            var r = Camera.main.transform.right; r.y = 0; r.Normalize();
            dir = r * input.x + f * input.y;
        }

        Vector3 delta = dir * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + delta);
        return dir;
    }

    private void Jump()
    {
        float force = condition.GetValue(ConditionType.JumpPower);
        Vector3 v = rb.velocity;
        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View2D)
            rb.velocity = new Vector3(v.x, 0f, 0f);
        else
            rb.velocity = new Vector3(v.x, 0f, v.z);

        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }

    public PlayerInputHandler Input => inputHandler;
}
