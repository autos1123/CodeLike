using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakePassiveItem : UIBase
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button takeButton;
    [SerializeField] private Button exitButton;

    private ItemData currentItem;
    private Inventory playerInventory;

    public override string UIName => this.GetType().Name;

    private void Start()
    {
        takeButton.onClick.AddListener(OnTake);
        exitButton.onClick.AddListener(Close);
    }

    public void Open(ItemData item, Inventory inventory)
    {
        base.Open();

        currentItem = item;
        playerInventory = inventory;

        itemIcon.sprite = Resources.Load<Sprite>(item.IconPath);
        descriptionText.text = item.description;
        
        GameEvents.TriggerRandomItemUIOpened();
    }

    private void OnTake()
    {
        if (currentItem == null || playerInventory == null)
        {
            Debug.LogWarning("[TakePassiveItem] 아이템 또는 인벤토리 없음");
            return;
        }

        playerInventory.AddToInventory(currentItem);
        Close();
    }
}
