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
    private void Awake()
    {
        slotUI = GetComponent<SlotUI>();
    }

    /// <summary>
    /// 마우스가 슬롯에 들어왔을 때 툴팁 표시
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"SlotPointer");         
        if (slotUI.backgroundImage != null)
        {
            slotUI.backgroundImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
        if (slotUI.slotType == SlotType.ActiveItem) return;

        if (slotUI.InventorySlot == null || slotUI.InventorySlot.IsEmpty) return;

        var item = slotUI.InventorySlot.InventoryItem;
        if (item == null) return;

        var tooltip = UIManager.Instance.GetUI<TooltipUI>();
        if (tooltip != null)
        {
            bool isEqiupSlot = slotUI.slotType == SlotType.Equip;
            tooltip.Show(item, eventData.position,isEqiupSlot);
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
        var tooltip = UIManager.Instance.GetUI<TooltipUI>();
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }
}
