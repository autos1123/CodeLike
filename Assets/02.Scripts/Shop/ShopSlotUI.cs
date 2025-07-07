using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

/// <summary>
/// 상점 슬롯 UI 요소. 아이템 정보 표시, 클릭 선택 처리, 비활성화 상태 표시를 담당.
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
    /// 슬롯 클릭 시 선택 상태 토글 및 배경색 변경
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isInteractable)
            return;
        // 장착중인 아이템인지 확인 (플레이어 슬롯일 때만)
        if (isPlayerSlot && !itemSlot.IsInvenSlotEmpty && shopUI.IsEquippedSlot(itemSlot))
        {
            ConfirmPopup.Show(
                "장착한 아이템입니다. 장착 해제하시겠습니까?",
                onConfirm: () =>
                {
                    SelectSlot();
                    EquipmentManager.Instance.UnEquip(itemSlot);
                    
                    var shopUI = UIManager.Instance.GetUI<ShopUI>();
                    if (shopUI != null)
                    {
                        shopUI.RefreshAllUI(); // 판매 슬롯 다시 구성하고 가격 재계산
                    }
                    else
                    {
                        Debug.LogWarning("[ConfirmPopup] ShopUI를 찾을 수 없습니다.");
                    }
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
    public void ForceSelect()
    {
        isSelected = true;
        BackgroundImage.color = selectedColor;
    }
}
