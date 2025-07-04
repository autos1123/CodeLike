using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI priceText;
    public Toggle selectionToggle;

    public ShopUI shopUI;

    private ItemSlot itemSlot;
    private bool isPlayerSlot;

    public bool IsSelected => selectionToggle != null && selectionToggle.isOn;
    public ItemSlot ItemSlot => itemSlot;

    public void Set(ItemSlot slot, bool isPlayerSlot)
    {
        this.itemSlot = slot;
        this.isPlayerSlot = isPlayerSlot;

        Refresh();

        selectionToggle.onValueChanged.AddListener(_ => 
        {
            shopUI?.UpdateTotalPrices();
        });
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

        int price = isPlayerSlot ? itemSlot.Item.sellPrice : itemSlot.Item.buyPrice;
        priceText.text = $"{price} G";
    }
    
}
