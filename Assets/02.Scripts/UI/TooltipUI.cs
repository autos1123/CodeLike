using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipUI:UIBase
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject equipText;
    [SerializeField] private RectTransform tooltipRect;

    public override string UIName => this.GetType().Name;

    private void Awake()
    {
        Close(); // 시작 시 비활성화
    }

    /// <summary>
    /// 툴팁 표시
    /// </summary>
    public void Show(ItemData itemData, Vector2 position, bool isEquipSlot)
    {
        if (itemData == null)
        {
            Hide();
            return;
        }

        nameText.text = itemData.name;
        nameText.color = GetColorByRarity(itemData.Rarity);
        descriptionText.text = itemData.description;
        
        if (equipText != null)
            equipText.SetActive(isEquipSlot);
        
        Open();
        UpdatePosition(position);
    }

    /// <summary>
    /// 위치 갱신
    /// </summary>
    public void UpdatePosition(Vector2 position)
    {
        if (tooltipRect == null || !gameObject.activeSelf) return;
        tooltipRect.position = position;
    }

    /// <summary>
    /// 툴팁 숨김
    /// </summary>
    public void Hide() => Close();
    
    private Color GetColorByRarity(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => Color.white,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.cyan,
            Rarity.Epic => Color.red,
            _ => Color.white
        };
    }
}
