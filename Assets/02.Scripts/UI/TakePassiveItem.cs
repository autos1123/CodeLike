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
    private PassiveItemBox sourceItemBox;
    public override string UIName => this.GetType().Name;

    private void Start()
    {
        takeButton.onClick.AddListener(OnTake);
        exitButton.onClick.AddListener(Close);
    }

    public void Open(ItemData item, Inventory inventory,PassiveItemBox itemBox)
    {
        base.Open();

        currentItem = item;
        playerInventory = inventory;
        sourceItemBox = itemBox;
        
        itemIcon.sprite = Resources.Load<Sprite>(item.IconPath);
        descriptionText.text = item.description;
        
        GameEvents.TriggerPassiveItemUIOpened();
    }

    private void OnTake()
    {
        if (currentItem == null || playerInventory == null)
        {
            Debug.LogWarning("[TakePassiveItem] 아이템 또는 인벤토리 없음");
            return;
        }

        playerInventory.AddToInventory(currentItem);
        if (sourceItemBox != null)
        {
            InteractionController interactionController = GameManager.Instance.Player.GetComponent<InteractionController>();
            if (interactionController != null)
            {
                interactionController.SetInteractTextParentToPlayer();
            }
            sourceItemBox.gameObject.SetActive(false);
            Destroy(sourceItemBox.gameObject); 
        }
        Close();
    }
}
