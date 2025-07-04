using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ShopSlotUI : MonoBehaviour,IPointerClickHandler
{
    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI blockText;
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
    
    public void Set(ItemSlot slot, bool isPlayerSlot,ShopUI shopUI)
    {
        this.itemSlot = slot;
        this.isPlayerSlot = isPlayerSlot;
        this.shopUI = shopUI;
        isSelected = false;
        isInteractable = true;
        Refresh();
    }

    private void Refresh()
    {
        if (itemSlot == null || itemSlot.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
            priceText.text = "";
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = Resources.Load<Sprite>(itemSlot.Item.IconPath);
        quantityText.text = itemSlot.Quantity > 1 ? itemSlot.Quantity.ToString() : "";

        priceText.text = (isPlayerSlot ? itemSlot.Item.sellPrice : itemSlot.Item.buyPrice) + " G";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isInteractable)
            return;
        
        isSelected = !isSelected;
        BackgroundImage.color = isSelected ? selectedColor : defaultColor;
        shopUI.UpdateTotalPrices();
    }
    
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        isSelected = false;
        BackgroundImage.color = interactable ? defaultColor : disabledColor;
        blockText.gameObject.SetActive(!interactable);
    }
}
