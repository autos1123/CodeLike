using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakePassiveItem : UIBase
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button takeButton;
    [SerializeField] private Button exitButton;

    private ItemData currentItem;
    private Inventory playerInventory;
    private PassiveItemBox sourceItemBox;
    
    private List<ItemData> rollingItems; // 아이콘 돌아갈 목록
    public override string UIName => this.GetType().Name;

    private void Start()
    {
        takeButton.onClick.AddListener(OnTake);
        exitButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public void Open(ItemData item, Inventory inventory,PassiveItemBox itemBox)
    {
        base.Open();
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"BoxOpen");
        currentItem = item;
        playerInventory = inventory;
        sourceItemBox = itemBox;
        
        GameEvents.TriggerPassiveItemUIOpened();
        //연출이 끝나면 활성화
        takeButton.gameObject.SetActive(false);

        if(!sourceItemBox.HasPlayedAnimation)
        {
            // itemBox의 possibleItemIds 가져와서 rollingItems 구성
            rollingItems = new List<ItemData>();
            foreach(int id in sourceItemBox.GetPossibleItemIds())
            {
                var data = sourceItemBox.itemDataTable.GetDataByID(id);
                if(data != null)
                    rollingItems.Add(data);
            }

            // 슬롯머신 연출 시작
            StartCoroutine(StartSlotRolling());
        }
        else
        {
            // 이미 연출한 박스: 바로 결과 보여줌
            itemIcon.sprite = Resources.Load<Sprite>(currentItem.IconPath);
            nameText.text = currentItem.name;
            descriptionText.text = currentItem.description;
            rarityText.text = currentItem.Rarity.ToString();
            rarityText.color = GetColorByRarity(currentItem.Rarity);
            takeButton.gameObject.SetActive(true);
        }
        
        GameManager.Instance.setState(GameState.Stop);
        // itemIcon.sprite = Resources.Load<Sprite>(item.IconPath);
        // nameText.text = item.name;
        // descriptionText.text = item.description;
        //
        // rarityText.text = item.Rarity.ToString();
        // rarityText.color = GetColorByRarity(item.Rarity);
        //
        // GameEvents.TriggerPassiveItemUIOpened();
    }

    public override void Close()
    {
        base.Close();
        nameText.text = "";
        descriptionText.text = "";
        rarityText.text = "";
        GameManager.Instance.setState(GameState.Play);
    }

    private IEnumerator StartSlotRolling()
    {
        float rollTime = 2f;
        float interval = 0.05f;
        float elapsed = 0f;
        int index = 0;

        while (elapsed < rollTime)
        {
            var currentItemData = rollingItems[index];
            itemIcon.sprite = Resources.Load<Sprite>(currentItemData.IconPath);

            index = (index + 1) % rollingItems.Count;
            elapsed += interval;

            interval += 0.005f; // 점점 느려지는 효과
            yield return new WaitForSeconds(interval);
        }

        // 최종 아이템에서 멈춤
        itemIcon.sprite = Resources.Load<Sprite>(currentItem.IconPath);
        nameText.text = currentItem.name;
        descriptionText.text = currentItem.description;
        rarityText.text = currentItem.Rarity.ToString();
        rarityText.color = GetColorByRarity(currentItem.Rarity);

        takeButton.gameObject.SetActive(true); // 선택 버튼 활성화
        
        sourceItemBox.MarkAnimationPlayed();
    }
    private Color GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return Color.white;
            case Rarity.Uncommon:
                return Color.green;
            case Rarity.Rare:
                return Color.cyan;
            case Rarity.Epic:
                return Color.red;
            default:
                return Color.white;
        }
    }
    private void OnTake()
    {
        if (currentItem == null || playerInventory == null)
        {
            Debug.LogWarning("[TakePassiveItem] 아이템 또는 인벤토리 없음");
            return;
        }
        
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"GetItem");

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
    private void OnCloseButtonClicked()
    {
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"BoxClose");
        Close();
    }
}
