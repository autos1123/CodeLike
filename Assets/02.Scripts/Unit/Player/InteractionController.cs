using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController:MonoBehaviour
{
    private PlayerController player;
    private PlayerInputHandler inputHandler;
    private Collider col;

    [Header("Interaction")]
    private IInteractable interactableObj;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableRange = 2.0f;
    [SerializeField] private Transform interactTextTr;
    private TextMeshProUGUI interactText;

    private Vector3 size_2D;

    //interactTextTr에 외부에서 접근할 수 있도록
    public Transform GetInteractTextTransform()
    {
        return interactTextTr;
    }
    private void Awake()
    {
        if(interactTextTr != null) // null 체크 추가
        {
            interactText = interactTextTr.GetComponentInChildren<TextMeshProUGUI>();
        }
        inputHandler = GetComponent<PlayerInputHandler>();
        col = GetComponent<Collider>(); // Collider 컴포넌트 가져오기
        player = GetComponent<PlayerController>(); // 플레이어 메쉬 트랜스폼 가져오기
    }
    private void OnEnable()
    {
        if(inputHandler != null)
        {
            inputHandler.OnInteraction += OnInteractableAction;
        }

        InvokeRepeating(nameof(InteractableCheck), 0f, 0.2f);
    }

    private void OnDisable()
    {
        if(inputHandler != null)
        {
            inputHandler.OnInteraction -= OnInteractableAction;
        }

        CancelInvoke(nameof(InteractableCheck));
    }

    /// <summary>
    /// 상호작용 키 입력 시 호출되는 메서드
    /// 플레이어 중심으로 상호작용 오브젝트 탐색 후 상호작용 메서드 호출
    /// </summary>
    /// <param name="context"></param>
    public void OnInteractableAction(InputAction.CallbackContext context)
    {
        // 상호작용 오브젝트가 없거나, 비활성화된 경우 무시
        if(interactableObj == null)
            return;

        if(interactableObj.CanInteract(gameObject))
            interactableObj.Interact(gameObject);
    }

    private void InteractableCheck()
    {
        if(interactTextTr == null) return; // 텍스트 Transform이 없으면 더 이상 진행하지 않음
        // 상호작용 오브젝트 탐색
        Collider[] hitColliders;

        if(ViewManager.Instance.CurrentViewMode == ViewModeType.View3D)
        {
            hitColliders = Physics.OverlapSphere(
                transform.position,
                interactableRange,
                interactableLayer
            );
        }
        else
        {
            size_2D = new Vector3(interactableRange, col.bounds.extents.y, col.bounds.extents.z);
            hitColliders = Physics.OverlapBox(col.bounds.center, size_2D, Quaternion.identity, interactableLayer);
        }

        IInteractable currentBestInteractable = null;
        float minDistanceSq = float.MaxValue;
        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].TryGetComponent(out IInteractable interactable))
            {
                // 오브젝트가 유효한지 확인
                if((interactable as MonoBehaviour) != null)
                {
                    float distSq = (hitColliders[i].transform.position - transform.position).sqrMagnitude;
                    if(distSq < minDistanceSq)
                    {
                        minDistanceSq = distSq;
                        currentBestInteractable = interactable;
                    }
                }
            }
        }
        // 텍스트 표시/숨기기 및 위치 업데이트 로직
        if(currentBestInteractable != null)
        {
            interactableObj = currentBestInteractable;
            SetInteractTextTransform(interactableObj.PromptPivot, interactableObj.InteractionPrompt);
            interactTextTr.gameObject.SetActive(true); // 활성화
        }
        else // 범위 내에 상호작용할 오브젝트가 없음
        {
            if(interactableObj != null) // 이전에 상호작용 중인 오브젝트가 있었다면
            {
                interactableObj = null; // 참조 해제
            }
            // 범위 밖이거나, 이미 파괴된 오브젝트가 있었다면 숨김
            if(interactTextTr.gameObject.activeSelf) // 이미 비활성화되어 있지 않은 경우에만
            {
                SetInteractTextTransform(transform);
                interactTextTr.gameObject.SetActive(false);
            }
        }
    }

    private void SetInteractTextTransform(Transform parent, string text = "")
    {
        // Null 체크 추가
        if(interactTextTr == null || parent == null || parent.gameObject == null)
        {
            if(interactTextTr != null) interactTextTr.gameObject.SetActive(false); // 숨기기
            return;
        }
        interactTextTr.SetParent(parent,false);
        interactTextTr.localPosition = Vector3.zero;
        interactText.text = text;
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            // 현재 뷰 모드에 따라 다른 방식으로 Gizmos를 그립니다.
            if(ViewManager.Instance.CurrentViewMode == ViewModeType.View3D)
            {
                Gizmos.DrawWireSphere(transform.position, interactableRange);
            }
            else
            {
                // 2D 모드에서는 박스 형태로 그리기
                Gizmos.DrawWireCube(col.bounds.center, size_2D * 2);
            }
        }
    }
}
