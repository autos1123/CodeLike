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

    private void Awake()
    {
        interactText = interactTextTr.GetComponentInChildren<TextMeshProUGUI>();
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
        // 상호작용 오브젝트 탐색
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            interactableRange,
            interactableLayer
        );

        if(hitColliders.Length == 0)
        {
            if(interactableObj != null)
            {
                interactableObj = null;
                SetInteractTextTransform(transform);
                interactTextTr.gameObject.SetActive(false);
            }
            return;
        }

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if(hitColliders[i].TryGetComponent(out IInteractable interactable))
            {
                interactableObj = interactable;
                SetInteractTextTransform(interactableObj.PromptPivot, interactableObj.InteractionPrompt);
                interactTextTr.gameObject.SetActive(true);
                return; // 첫 번째 상호작용만 처리
            }
        }
    }

    private void SetInteractTextTransform(Transform parent, string text = "")
    {
        interactTextTr.parent = parent;
        interactTextTr.localPosition = Vector3.zero;
        interactText.text = text;
    }
}
