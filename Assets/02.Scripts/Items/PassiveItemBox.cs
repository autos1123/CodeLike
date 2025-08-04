using UnityEngine;

public class PassiveItemBox : MonoBehaviour,IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 열기";
    [SerializeField] private Transform promptPivot;
    [SerializeField] private ItemDataTable itemDataTable;
    
    private List<int> possibleItemIds = new List<int>(); // 랜덤 대상 ID 목록

    private int? fixedPicekedId; //한번 뽑히면 저장되는 아이템id값
    
    public string InteractionPrompt => interactionPrompt;
    public Transform PromptPivot => promptPivot;

    public bool CanInteract(GameObject interactor)
    {
        return interactor.GetComponent<Inventory>() != null;
    }

    public void Interact(GameObject interactor)
    {
        var inventory = interactor.GetComponent<Inventory>();
        if (inventory == null) return;

        if (possibleItemIds == null || possibleItemIds.Length == 0)
        {
            Debug.LogWarning("[PassiveItemBox] 아이템 ID 목록이 비어 있습니다.");
            return;
        }

        if(fixedPicekedId == null)
        {
            int randomIndex = Random.Range(0, possibleItemIds.Length);
            fixedPicekedId = possibleItemIds[randomIndex];
        }
        int pickedID = fixedPicekedId.Value;
        var item = TableManager.Instance.GetTable<ItemDataTable>()?.GetDataByID(pickedID);
        if (item == null)
        {
            Debug.LogError($"[PassiveItemBox] ID {fixedPicekedId}에 해당하는 아이템을 찾을 수 없습니다.");
            return;
        }

        var takePassiveItemUI = UIManager.Instance.GetUI<TakePassiveItem>();
        takePassiveItemUI.Open(item, inventory,this);
    }
}
