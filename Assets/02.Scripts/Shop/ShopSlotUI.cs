using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

/// <summary>
/// 상점 슬롯 UI 컴포넌트. 아이템 아이콘/가격 출력, 클릭 선택/장착 해제 처리 및 선택 상태 유지 기능을 담당.
/// </summary>
public class ShopSlotUI : MonoBehaviour,IPointerClickHandler
{
    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI blockText;
    public TextMeshProUGUI equipText;
    public Image BackgroundImage;
    
    private ShopUI shopUI;

    private ItemSlot itemSlot;
    private bool isPlayerSlot;
    private bool isSelected;
    private bool isInteractable = true;

    public Color defaultColor = Color.white;
    public Color selectedColor = Color.red;
    public Color disabledColor = Color.gray;
    public bool IsSelected => isSelected;
    public ItemSlot ItemSlot => itemSlot;
    
    /// <summary>
    /// 아이템 슬롯 UI 초기화: 아이템 정보, 소유자 타입, 참조 설정
    /// </summary>
    public void Set(ItemSlot slot, bool isPlayerSlot,ShopUI shopUI)
    {
        this.itemSlot = slot;
        this.isPlayerSlot = isPlayerSlot;
        this.shopUI = shopUI;
        isSelected = false;
        isInteractable = true;
        // 장착중인 슬롯이면 equipText 활성화
        if (isPlayerSlot && shopUI.IsEquippedSlot(itemSlot))
        {
            equipText.gameObject.SetActive(true);
        }
        else
        {
            equipText.gameObject.SetActive(false);
        }
        Refresh();
    }
    
    /// <summary>
    /// 슬롯 UI를 새로 그리며 아이콘, 가격, 수량 표시 업데이트
    /// </summary>
    private void Refresh()
    {
        if (itemSlot == null || itemSlot.IsInvenSlotEmpty)
        {
            iconImage.enabled = false;
            priceText.text = "";
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = Resources.Load<Sprite>(itemSlot.Item.IconPath);
        
        priceText.text = (isPlayerSlot ? itemSlot.Item.sellPrice : itemSlot.Item.buyPrice) + " G";
    }
    
    /// <summary>
    /// 슬롯 클릭 시 장착 해제 여부를 체크하고, 선택 또는 해제 팝업을 실행
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isInteractable)
            return;
        // 장착중인 아이템인지 확인 (플레이어 슬롯일 때만)
        if (isPlayerSlot && !itemSlot.IsInvenSlotEmpty && shopUI.IsEquippedSlot(itemSlot))
        {
            // 현재 슬롯 + 아이템 기억해둠
            var rememberedSlot = itemSlot;
            var rememberedItem = itemSlot.Item;

            UIManager.Instance.ShowConfirmPopup(
                "장착한 아이템입니다. 장착 해제하시겠습니까?",
                onConfirm: () =>
                {
                    var shopUI = UIManager.Instance.GetUI<ShopUI>();
                    shopUI.RememberSelectedItem(itemSlot);
                    EquipmentManager.Instance.UnEquip(itemSlot);
                    shopUI?.RefreshAllUI();
                },
                onCancel: () =>
                {
                    Debug.Log("선택 취소됨");
                }
            );
        }
        else
        {
            SelectSlot();
        }
    }
    
    /// <summary>
    /// 슬롯을 선택 불가능한 상태로 변경하고 배경/텍스트를 회색 처리
    /// </summary>
    /// <param name="interactable">선택 가능 여부</param>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        isSelected = false;
        BackgroundImage.color = interactable ? defaultColor : disabledColor;
        blockText.gameObject.SetActive(!interactable);
    }
    /// <summary>
    /// 선택 상태 토글 및 UI 반영, 선택된 슬롯을 ShopUI에 등록/해제
    /// </summary>
    private void SelectSlot()
    {
        isSelected = !isSelected;
        BackgroundImage.color = isSelected ? selectedColor : defaultColor;
        if (isPlayerSlot)
        {
            if (isSelected)
                shopUI.RememberSelectedItem(itemSlot); // 선택 기억
            else
                shopUI.ForgetSelectedItem(itemSlot);   // 선택 해제
        }
        shopUI.UpdateTotalPrices();
    }
    /// <summary>
    /// 강제로 선택된 상태로 설정 (색상 변경만 적용됨)
    /// </summary>
    public void ForceSelect()
    {
        isSelected = true;
        BackgroundImage.color = selectedColor;
    }
}
