using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    
    public void Set(ItemSlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            quantityText.text = slot.Quantity.ToString();
        }
    }
}
