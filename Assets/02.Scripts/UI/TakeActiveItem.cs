using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakeActiveItem:UIBase
{
    [Header("각 슬롯(이미지+버튼)")]
    [SerializeField] private Image[] slotImages; 
    [SerializeField] private Button[] slotButtons; 

    [Header("왼쪽 큰 아이콘 (선택 슬롯)")]
    [SerializeField] private Image leftItemImage;

    [Header("오른쪽 큰 아이콘 (획득 아이템)")]
    [SerializeField] private Image acquiredItemImage;

    [Header("왼쪽 큰 설명 (선택 슬롯)")]
    [SerializeField] private TextMeshProUGUI leftItemDescription;

    [Header("오른쪽 큰 설명 (획득 아이템)")]
    [SerializeField] private TextMeshProUGUI acquiredItemDescription;

    [Header("획득하기 버튼")]
    [SerializeField] private Button takeButton;

    [Header("나가기 버튼")]
    [SerializeField] private Button closeButton;


    // 내부 데이터
    private ActiveItemSlot[] activeItemSlots;
    private ActiveItemData acquiredItem;
    private int selectedSlot = -1;
    private Inventory playerInventory;
    private ActiveItemBox sourceActiveItemBox; 
    public override string UIName => this.GetType().Name;

    private void Start()
    {
        for(int i = 0; i < slotButtons.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(idx));
        }
        takeButton.interactable = false;
        closeButton.onClick.AddListener(OnCloseButtonClicked); // 나가기 버튼 이벤트 등록
    }

    public void Open(ActiveItemSlot[] _activeItemSlots, ActiveItemData _acquiredItem, Inventory _playerInventory = null,ActiveItemBox _sourceActiveItemBox = null)
    {
        base.Open();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"BoxOpen");

        activeItemSlots = _activeItemSlots;
        acquiredItem = _acquiredItem;
        playerInventory = _playerInventory;
        sourceActiveItemBox = _sourceActiveItemBox;
        for(int i = 0; i < activeItemSlots.Length; i++)
        {
            var itemData = activeItemSlots[i].ActiveItem;
            slotImages[i].sprite = GetIcon(itemData);
            slotImages[i].color = new Color32(0, 0, 0, 0);
        }

        selectedSlot = -1;
        takeButton.interactable = false;

        leftItemImage.sprite = GetIcon(activeItemSlots[0].ActiveItem);
        if(leftItemImage.sprite == null) leftItemImage.color = new Color32(0, 0, 0, 0);
        leftItemDescription.text = GetDescriptionByID(activeItemSlots[0].ActiveItem?.ID ?? -1);

        acquiredItemImage.sprite = GetIcon(acquiredItem);
        acquiredItemDescription.text = GetDescriptionByID(acquiredItem?.ID ?? -1);
        
        GameEvents.TriggerActiveItemUIOpened();
    }

    private void OnSlotSelected(int slotIndex)
    {
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"ShopSlotClick");
        selectedSlot = slotIndex;
        
        for(int i = 0; i < slotImages.Length; i++)
            slotImages[i].color = (i == slotIndex) ? new Color32(255, 100, 0, 255) : new Color32(0, 0, 0, 0);

        var selectedItem = activeItemSlots[slotIndex].ActiveItem;
        leftItemImage.sprite = GetIcon(selectedItem);
        leftItemDescription.text = GetDescriptionByID(selectedItem?.ID ?? -1);

        // 오른쪽(획득 아이템)은 고정
        acquiredItemImage.sprite = GetIcon(acquiredItem);
        acquiredItemDescription.text = GetDescriptionByID(acquiredItem?.ID ?? -1);

        takeButton.interactable = true;
    }

    public void OnTakeButton()
    {
        if(selectedSlot == -1) return;
        
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"GetItem");

        var slotObj = activeItemSlots[selectedSlot];

        Skillinput targetSkill = (Skillinput)selectedSlot;

        if(slotObj.IsEmpty)
        {
            playerInventory?.AddtoActiveSlot(selectedSlot, acquiredItem, targetSkill);
        }
        else
        {
            playerInventory?.ReplaceActiveSlot(selectedSlot, acquiredItem, targetSkill);
        }

        UIManager.Instance.GetUI<HUD>()?.UpdateActiveItemIcons();
        if (sourceActiveItemBox != null)
        {
            InteractionController interactionController = GameManager.Instance.Player.GetComponent<InteractionController>();
            if (interactionController != null)
            {
                interactionController.SetInteractTextParentToPlayer();
            }
            sourceActiveItemBox.gameObject.SetActive(false);
            Destroy(sourceActiveItemBox.gameObject);
        }
        Close();
    }
    private void OnCloseButtonClicked()
    {
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"BoxClose");
        Close();
    }
    
    Sprite GetIcon(ActiveItemData item)
    {
        return item == null ? null : Resources.Load<Sprite>(item.IconPath);
    }

    string GetDescriptionByID(int id)
    {
        if(id < 0) return "";
        var table = TableManager.Instance.GetTable<ActiveItemDataTable>();
        var item = table?.GetDataByID(id);
        return item?.description ?? "";
    }
}
