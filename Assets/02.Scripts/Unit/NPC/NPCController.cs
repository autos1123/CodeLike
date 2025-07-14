using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    public int ID;

    [SerializeField] private string interactionPrompt = "[F] 대화하기";
    [SerializeField] private Transform promptPivot;

    public string InteractionPrompt => interactionPrompt;

    public Transform PromptPivot => promptPivot;

    public bool CanInteract(GameObject interactor)
    {
        return true;
    }

    public void Interact(GameObject interactor)
    {
        var npcDataTable = TableManager.Instance.GetTable<NPCDataTable>();
        var npcData = npcDataTable.GetDataByID(ID);

        if (npcData == null)
        {
            Debug.LogWarning($"[NPCController] ID {ID}에 해당하는 NPCData를 찾을 수 없습니다.");
            return;
        }

        switch (npcData.Type)
        {
            case NPCType.Normal:
                OpenDialogue(interactor);
                break;

            case NPCType.Merchant:
                TryOpenShop();
                break;
        }
    }
    private void OpenDialogue(GameObject interactor)
    {
        UIManager.Instance.ShowUI<DialogueBoard>();
        DialogueManager.Instance.onDialogue(interactor.transform, this.transform);
    }
    
    private void TryOpenShop()
    {
        if (!TryGetComponent(out ShopInventory shopInventory))
        {
            Debug.LogWarning($"[NPCController] 이 NPC({ID})에 ShopInventory 컴포넌트가 없습니다.");
            return;
        }

        if (!UIManager.Instance.TryGetUI<ShopUI>(out var shopUI))
        {
            Debug.LogError("[NPCController] ShopUI를 찾을 수 없습니다.");
            return;
        }

        if (!shopInventory.Initialized)
        {
            shopInventory.OnInitialized += () => shopUI.OpenWithInventory(shopInventory);
        }
        else
        {
            shopUI.OpenWithInventory(shopInventory);
        }
    }
}
