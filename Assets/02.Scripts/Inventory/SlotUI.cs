using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    Inventory,
    Equip,
    ActiveItem
}
/// <summary>
/// 하나의 인벤토리 또는 장비 슬롯을 표현하는 UI 컴포넌트
/// 아이템의 아이콘, 수량 표시 및 툴팁, 드래그 앤 드롭 기능을 처리
/// </summary>
public class SlotUI : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IDropHandler,IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public SlotType slotType;
    
    public Image iconImage;
    [CanBeNull] public TextMeshProUGUI EquipText;
    private InventoryUI inventoryUI;
    public ItemSlot ItemSlot { get; private set; } 
    
    private bool isBeingDragged = false;
    public void Init(InventoryUI ui)
    {
        inventoryUI = ui;
    }
    /// <summary>
    /// 슬롯 데이터에 따라 UI를 갱신
    /// </summary>
    public void Set(ItemSlot slot)
    {
        ItemSlot = slot;
        RefreshVisual();
    }
    
    /// <summary>
    /// 현재 슬롯 데이터에 따라 아이콘과 수량 등의 시각적 요소를 갱신
    /// </summary>
    private void RefreshVisual()
    {
        if (ItemSlot == null)
            return;
        //액티브 아이템
        if (slotType == SlotType.ActiveItem)
        {
            if (ItemSlot.ActiveItem == null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
                return;
            }

            iconImage.sprite = Resources.Load<Sprite>(ItemSlot.ActiveItem.IconPath);
            iconImage.enabled = true;
            iconImage.color = isBeingDragged ? new Color(1, 1, 1, 0.3f) : Color.white;

            return;
        }

        // 일반 아이템 (ItemData)
        if (ItemSlot.IsInvenSlotEmpty)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            
            // 장비 텍스트 비활성화
            if (slotType == SlotType.Equip && EquipText != null)
            {
                EquipText.gameObject.SetActive(false);
            }
            
            return;
        }

        iconImage.sprite = Resources.Load<Sprite>(ItemSlot.Item.IconPath);
        iconImage.enabled = true;
        iconImage.color = isBeingDragged ? new Color(1, 1, 1, 0.3f) : Color.white;
        
        // 장비 텍스트 활성화
        if (slotType == SlotType.Equip && EquipText != null)
        {
            EquipText.gameObject.SetActive(true);
        }
    }
    
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotType == SlotType.ActiveItem || ItemSlot == null || ItemSlot.IsInvenSlotEmpty)
        {
            eventData.pointerDrag = null;
            return;
        }
        isBeingDragged = true;
        iconImage.raycastTarget = false;
        TooltipManager.Instance.Hide();
        
        RefreshVisual();
        DragManager.Instance.CreateGhost(iconImage.sprite);
    }
    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemSlot == null || ItemSlot.IsInvenSlotEmpty)
            return;

        DragManager.Instance.UpdateGhostPosition(eventData.position);
    }

    // 드래그 끝 (드롭 여부와 관계없이)
    public void OnEndDrag(PointerEventData eventData)
    {
        iconImage.raycastTarget = true;
        isBeingDragged = false;
        
        RefreshVisual();
        DragManager.Instance.ClearGhost();
    }

    // 드롭 처리
    public void OnDrop(PointerEventData eventData)
    {
        SlotUI draggedSlotUI = eventData.pointerDrag?.GetComponent<SlotUI>();
        if (draggedSlotUI == null || draggedSlotUI == this || draggedSlotUI.ItemSlot == null)
            return;
        
        inventoryUI.HandleSlotSwap(this, draggedSlotUI);
        
    }
    
    /// <summary>
    /// 마우스가 슬롯 위에 올라갔을 때 호출, 툴팁 표시
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotType == SlotType.ActiveItem) return; // 액티브아이템슬롯은 리턴

        if (ItemSlot == null || ItemSlot.IsInvenSlotEmpty) return;
        
        // 툴팁 중복 호출 방지
        if (!TooltipManager.Instance.IsVisible)
            TooltipManager.Instance.Show(ItemSlot.GetDescription(slotType), eventData.position);

    }

    /// <summary>
    /// 마우스가 슬롯에서 벗어났을 때 호출, 툴팁 숨김
    /// </summary>
    /// <param name="eventData">이벤트 데이터</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
    
    /// <summary>
    /// 슬롯 클릭시 (현재는 액티브아이템 슬롯만 적용)
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");

        // 액티브 아이템 슬롯만 infoText 표시
        if (slotType != SlotType.ActiveItem || ItemSlot == null || ItemSlot.ActiveItem == null)
            return;
        
        string description = ItemSlot.GetDescription(slotType);
        inventoryUI?.SetInfoText(description);
        
    }
}
