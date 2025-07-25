using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 슬롯의 드래그 앤 드롭 기능만을 담당하는 컴포넌트
/// 드래그 시작, 이동, 종료 및 드롭 처리까지 분리하여 처리
/// </summary>
[RequireComponent(typeof(SlotUI))]
public class SlotDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private SlotUI slotUI;
    private Image iconImage;
    [SerializeField]private TooltipManager tooltipManager;
    [SerializeField]private DragManager dragManager;
    private void Awake()
    {
        slotUI = GetComponent<SlotUI>();
        iconImage = slotUI.iconImage;
    }

    private void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
        dragManager = FindObjectOfType<DragManager>();
    }
    /// <summary>
    /// 드래그 시작 시 호출
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotUI.slotType == SlotType.ActiveItem || slotUI.InventorySlot == null || slotUI.InventorySlot.IsEmpty)
        {
            eventData.pointerDrag = null;
            return;
        }

        iconImage.raycastTarget = false;

        slotUI.RefreshVisual();
        tooltipManager.Hide();

        dragManager.CreateGhost(iconImage.sprite);
    }
    /// <summary>
    /// 드래그 중 위치 업데이트
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (slotUI.InventorySlot == null || slotUI.InventorySlot.IsEmpty)
            return;

        dragManager.UpdateGhostPosition(eventData.position);
    }
    /// <summary>
    /// 드래그 종료 시 호출
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        iconImage.raycastTarget = true;

        slotUI.RefreshVisual();
        dragManager.ClearGhost();
    }
    /// <summary>
    /// 슬롯 위에 다른 슬롯이 드롭됐을 때 처리
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlotUI = eventData.pointerDrag?.GetComponent<SlotUI>();
        if (draggedSlotUI == null || draggedSlotUI == slotUI || draggedSlotUI.InventorySlot == null)
            return;

        slotUI.inventoryUI.HandleSlotSwap(slotUI, draggedSlotUI);
    }
}