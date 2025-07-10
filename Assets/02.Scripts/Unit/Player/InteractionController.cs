using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    private PlayerInputHandler inputHandler;

    [Header("Interaction")]
    private IInteractable interactableObj;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float interactableRange = 2.0f;
    [SerializeField] private Transform interactTextTr;
    private TextMeshProUGUI interactText;
    
    //interactTextTr에 외부에서 접근할 수 있도록
    public Transform GetInteractTextTransform()
    {
        return interactTextTr;
    }
    private void Awake()
    {
        if (interactTextTr != null) // null 체크 추가
        {
            interactText = interactTextTr.GetComponentInChildren<TextMeshProUGUI>();
        }
        inputHandler = GetComponent<PlayerInputHandler>();
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

        if(interactableObj == null)
        {
            Debug.LogWarning("[InteractionController] No interactable in range!"); // 추가
            return;
        }
        if(interactableObj.CanInteract(gameObject))
            interactableObj.Interact(gameObject);
    }

    private void InteractableCheck()
    {
        if (interactTextTr == null) return; // 텍스트 Transform이 없으면 더 이상 진행하지 않음
        // 상호작용 오브젝트 탐색
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            interactableRange,
            interactableLayer
        );

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
        if (currentBestInteractable != null)
        {
            interactableObj = currentBestInteractable;
            SetInteractTextTransform(interactableObj.PromptPivot, interactableObj.InteractionPrompt);
            interactTextTr.gameObject.SetActive(true); // 활성화
        }
        else // 범위 내에 상호작용할 오브젝트가 없음
        {
            if (interactableObj != null) // 이전에 상호작용 중인 오브젝트가 있었다면
            {
                interactableObj = null; // 참조 해제
            }
            // 범위 밖이거나, 이미 파괴된 오브젝트가 있었다면 숨김
            if (interactTextTr.gameObject.activeSelf) // 이미 비활성화되어 있지 않은 경우에만
            {
                SetInteractTextTransform(transform); 
                interactTextTr.gameObject.SetActive(false); 
            }
        }
    }

    private void SetInteractTextTransform(Transform parent, string text = "")
    {
        // Null 체크 추가
        if (interactTextTr == null || parent == null || parent.gameObject == null)
        {
            if (interactTextTr != null) interactTextTr.gameObject.SetActive(false); // 숨기기
            return;
        }
        interactTextTr.parent = parent;
        interactTextTr.localPosition = Vector3.zero;
        interactText.text = text;
    }
}
