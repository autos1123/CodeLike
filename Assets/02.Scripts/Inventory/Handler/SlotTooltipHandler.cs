using System;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 슬롯에 마우스를 올렸을 때 툴팁을 표시,
/// 벗어났을 때 툴팁을 숨기는 역할을 담당
/// </summary>
[RequireComponent(typeof(SlotUI))]
public class SlotTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SlotUI slotUI;
    [SerializeField] private TooltipManager tooltipManager;
    private void Awake()
    {
        slotUI = GetComponent<SlotUI>();
    }

    private void Start()
    {
        tooltipManager = FindObjectOfType<TooltipManager>();
    }

    /// <summary>
    /// 마우스가 슬롯에 들어왔을 때 툴팁 표시
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotUI.backgroundImage != null)
        {
            slotUI.backgroundImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
        if (slotUI.slotType == SlotType.ActiveItem) return;

        if (slotUI.InventorySlot == null || slotUI.InventorySlot.IsEmpty) return;

        if (!tooltipManager.IsVisible)
        {
            tooltipManager.Show(slotUI.InventorySlot.GetDescription(slotUI.slotType), eventData.position);
        }
    }
    /// <summary>
    /// 마우스가 슬롯에서 나갔을 때 툴팁 숨김
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (slotUI.backgroundImage != null)
        {
            slotUI.backgroundImage.color = new Color(1f, 1f, 1f, 1f);
        }
        tooltipManager.Hide();
    }
}
