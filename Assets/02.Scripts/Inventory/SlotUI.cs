using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 슬롯 하나를 표현하는 클래스.
/// 아이콘 이미지 및 수량 텍스트를 설정.
/// </summary>
public class SlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    
    /// <summary>
    /// 슬롯 데이터에 따라 UI를 갱신
    /// </summary>
    /// <param name="slot">슬롯 데이터</param>
    public void Set(ItemSlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            quantityText.text = "";
        }
        else
        {
            iconImage.sprite = Resources.Load<Sprite>(slot.Item.IconPath);
            if(slot.Quantity > 1) // 겹치는 아이템만 수량 표시
            {
                quantityText.text = slot.Quantity.ToString();
            }
            else
            {
                quantityText.text = "";
            }
        }
    }
}
