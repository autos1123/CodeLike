using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    Inventory,
    Equip
}
/// <summary>
/// UI 슬롯 하나를 표현하는 클래스.
/// 아이콘 이미지 및 수량 텍스트를 설정.
/// </summary>
public class SlotUI : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public SlotType slotType;
    
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    
    public ItemSlot ItemSlot { get; private set; } 
    [HideInInspector] public Transform originalParent;  // 드래그용
    
    private bool isBeingDragged = false;
    
    /// <summary>
    /// 슬롯 데이터에 따라 UI를 갱신
    /// </summary>
    public void Set(ItemSlot slot)
    {
        ItemSlot = slot;
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (ItemSlot == null || ItemSlot.IsEmpty)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            quantityText.text = "";
            return;
        }

        iconImage.sprite = Resources.Load<Sprite>(ItemSlot.Item.IconPath);
        iconImage.enabled = true;

        // 드래그 중이면 흐리게 처리
        iconImage.color = isBeingDragged ? new Color(1, 1, 1, 0.3f) : Color.white;

        quantityText.text = ItemSlot.Quantity > 1 ? ItemSlot.Quantity.ToString() : "";
        quantityText.alpha = isBeingDragged ? 0.3f : 1f;
    }
    
    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemSlot == null || ItemSlot.IsEmpty)
        {
            eventData.pointerDrag = null;
            return;
        }

        iconImage.raycastTarget = false;
        isBeingDragged = true;
        RefreshVisual();

        DragManager.Instance.CreateGhost(iconImage.sprite);
    }
    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemSlot == null || ItemSlot.IsEmpty)
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
        
        SwapSlotData(draggedSlotUI);
        
    }
    
    /// <summary>
    /// 슬롯 데이터를 서로 교환
    /// </summary>
    private void SwapSlotData(SlotUI fromSlotUI)
    {
        if (ItemSlot == null || fromSlotUI.ItemSlot == null)
        {
            Debug.LogWarning("SlotUI: ItemSlot이 null입니다.");
            return;
        }

        var temp = new ItemSlot();
        temp.Set(ItemSlot.Item, ItemSlot.Quantity);

        ItemSlot.Set(fromSlotUI.ItemSlot.Item, fromSlotUI.ItemSlot.Quantity);
        fromSlotUI.ItemSlot.Set(temp.Item, temp.Quantity);

        Set(ItemSlot);
        fromSlotUI.Set(fromSlotUI.ItemSlot);
    }
}
