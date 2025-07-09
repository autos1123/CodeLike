using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakeActiveItem:UIBase
{
    [Header("각 슬롯(이미지+버튼)")]
    [SerializeField] private Image[] slotImages; // 슬롯 아이콘 (버튼과 같은 오브젝트)
    [SerializeField] private Button[] slotButtons; // 슬롯 버튼

    [Header("슬롯별 설명 텍스트")]
    [SerializeField] private TextMeshProUGUI[] slotDescriptions; // 슬롯 설명

    [Header("왼쪽 큰 아이콘 (선택 슬롯)")]
    [SerializeField] private Image leftItemImage;

    [Header("오른쪽 큰 아이콘 (획득 아이템)")]
    [SerializeField] private Image acquiredItemImage;

    [Header("획득하기 버튼")]
    [SerializeField] private Button takeButton;

    // 내부 데이터
    private ActiveItemSlot[] activeItemSlots; // 플레이어의 액티브 슬롯 참조
    private ActiveItemData acquiredItem; // 새로 얻은 아이템
    private int selectedSlot = -1; // 현재 선택 슬롯

    // (인벤토리 참조 필요 시 연결)
    private Inventory playerInventory;

    public override string UIName => "TakeActiveItem";

    private void Start()
    {
        // 슬롯 버튼 클릭 이벤트 등록
        for(int i = 0; i < slotButtons.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(idx));
        }
        // 획득하기 버튼은 슬롯 선택 전엔 비활성화
        takeButton.interactable = false;
    }

    /// <summary>
    /// UI를 열고 슬롯/획득 아이템 데이터로 초기화
    /// </summary>
    /// <param name="_activeItemSlots">플레이어 액티브 슬롯</param>
    /// <param name="_acquiredItem">새로 획득한 아이템</param>
    /// <param name="_playerInventory">플레이어 인벤토리(선택/필요시)</param>
    public void Open(ActiveItemSlot[] _activeItemSlots, ActiveItemData _acquiredItem, Inventory _playerInventory = null)
    {

        base.Open();
        Debug.Log($"[TakeActiveItem] Open 호출됨 - 슬롯수: {_activeItemSlots.Length}, 획득아이템: {_acquiredItem?.name}");
        activeItemSlots = _activeItemSlots;
        acquiredItem = _acquiredItem;
        playerInventory = _playerInventory;

        // 슬롯별 아이콘/설명 초기화
        for(int i = 0; i < activeItemSlots.Length; i++)
        {
            slotImages[i].sprite = GetIcon(activeItemSlots[i].ActiveItem);
            slotImages[i].color = Color.white;
            slotDescriptions[i].text = activeItemSlots[i].GetDescription();
        }

        selectedSlot = -1;
        takeButton.interactable = false;

        // 왼쪽 큰 아이콘: 기본 0번 슬롯(선택 안 됐을 때)
        leftItemImage.sprite = GetIcon(activeItemSlots[0].ActiveItem);

        // 오른쪽 큰 아이콘: 획득 아이템
        acquiredItemImage.sprite = GetIcon(acquiredItem);
    }

    /// <summary>
    /// 슬롯 선택 시 실행 (강조 및 왼쪽 아이콘 변경)
    /// </summary>
    private void OnSlotSelected(int slotIndex)
    {
        selectedSlot = slotIndex;

        for(int i = 0; i < slotImages.Length; i++)
            slotImages[i].color = (i == slotIndex) ? Color.yellow : Color.white;

        leftItemImage.sprite = GetIcon(activeItemSlots[slotIndex].ActiveItem);

        takeButton.interactable = true;
    }

    /// <summary>
    /// "획득하기" 버튼 클릭 시 실행
    /// </summary>
    public void OnTakeButton()
    {
        if(selectedSlot == -1) return;

        // 기존 아이템 임시 보관
        var oldItem = activeItemSlots[selectedSlot].ActiveItem;

        // 기존 아이템을 인벤토리에 넣지 않고 그냥 교체
        activeItemSlots[selectedSlot].Set(acquiredItem);

        this.Close();
    }


    /// <summary>
    /// 아이템 데이터에서 아이콘 스프라이트 반환
    /// </summary>
    Sprite GetIcon(ActiveItemData item)
    {
        return item == null ? null : Resources.Load<Sprite>(item.IconPath);
    }
}
