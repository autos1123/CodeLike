using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItemBox : MonoBehaviour,IInteractable
{
    [SerializeField] private string interactionPrompt = "[F] 열기";
    [SerializeField] private Transform promptPivot;
    [SerializeField] private int[] possibleItemIds; // 랜덤 대상 ID 목록

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

        int randomIndex = Random.Range(0, possibleItemIds.Length);
        int pickedID = possibleItemIds[randomIndex];
        var item = TableManager.Instance.GetTable<ItemDataTable>()?.GetDataByID(pickedID);
        if (item == null)
        {
            Debug.LogError($"[PassiveItemBox] ID {pickedID}에 해당하는 아이템을 찾을 수 없습니다.");
            return;
        }

        var takePassiveItemUI = UIManager.Instance.GetUI<TakePassiveItem>();
        takePassiveItemUI.Open(item, inventory);
    }
}
