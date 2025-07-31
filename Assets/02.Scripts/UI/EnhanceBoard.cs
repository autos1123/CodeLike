using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class EnhanceBoard : UIBase
{
    [SerializeField] EnhanceCard[] cards;
    TableManager _tableManager;
    
    // 각 EnhanceCard 인스턴스의 선택 상태를 저장할 변수 (선택된 카드가 하나만 있다고 가정)
    private EnhanceCard _selectedCardInstance = null; // 선택된 카드를 저장
    // EnhanceBoard를 연 NPCController의 ID를 저장할 변수
    private int _callingInstanceId;
    
    private Dictionary<int, List<EnhanceData>> _enhanceCacheByInstance = new();
    private Dictionary<int, Dictionary<EnhanceCard, bool>> _flippedStatesByInstance = new();
    private GameObject _callingNpcObject;
    
    public bool isEnhanceCompleted = false;
    public Button exitButton;
    public override string UIName => this.GetType().Name;

    void Start()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(Close);
        }
    }

    public override void Open()
    {
        base.Open();
        Open(gameObject);
    }
    public void Open(GameObject callingNpcObject)
    {
        base.Open();
        
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"EnhanceOpen");
        
        _callingNpcObject = callingNpcObject;
        _callingInstanceId = callingNpcObject.GetInstanceID();
        
        if(_tableManager == null) _tableManager = TableManager.Instance;
        
        if (!_enhanceCacheByInstance.TryGetValue(_callingInstanceId, out var npcEnhanceList))
        {
            npcEnhanceList = _tableManager.GetTable<EnhanceDataTable>().dataList
                .ShuffleWithSeed(_callingInstanceId)
                .Take(cards.Length)
                .ToList();

            _enhanceCacheByInstance[_callingInstanceId] = npcEnhanceList;
        }

        _selectedCardInstance = null;

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Clear();

            if (i < npcEnhanceList.Count)
            {
                cards[i].gameObject.SetActive(true);
                cards[i].init(npcEnhanceList[i], this);
            }
            else
            {
                cards[i].gameObject.SetActive(false);
            }
        }

        if (_flippedStatesByInstance.TryGetValue(_callingInstanceId, out var flippedDict))
        {
            foreach (var card in cards)
            {
                card.SetFlippedState(flippedDict.TryGetValue(card, out var flip) ? flip : false);
            }
        }
        else
        {
            foreach (var card in cards)
                card.SetFlippedState(false);
        }

        foreach (var card in cards)
        {
            card.SetInteractable(true);
            card.SetSelectButtonActive(false);
        }


        if (exitButton != null)
            exitButton.gameObject.SetActive(true);
    }
    // 각 EnhanceCard에서 카드가 뒤집혔음을 알릴 때 호출될 메소드
    public void CardFlipped(EnhanceCard card, bool isFlipped)
    {
        if (!_flippedStatesByInstance.ContainsKey(_callingInstanceId))
            _flippedStatesByInstance[_callingInstanceId] = new();

        _flippedStatesByInstance[_callingInstanceId][card] = isFlipped;
    }
    // 각 EnhanceCard에서 카드가 선택되었음을 알리는 메소드
    public void CardSelected(EnhanceCard selectcard)
    {
        _selectedCardInstance = selectcard;
        
        foreach(var card in cards)
        {
            if(card != selectcard)
            {
                card.SetSelectButtonActive(false); // 선택되지 않은 카드의 선택 버튼 비활성화
            }
        }
    }
    public override void Close()
    {
        if (!isEnhanceCompleted)
        {
            SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"EnhanceClose");
            foreach(var card in cards)
            {
                card.gameObject.SetActive(false); 
                card.SetInteractable(false); 
                card.SetSelectButtonActive(false);
            }
        }
        else // 카드를 선택하여 닫혔을 경우 (강화가 완료된 경우)
        {
            foreach (var card in cards)
                card.Clear();
            
            // 캐시 초기화
            _enhanceCacheByInstance.Remove(_callingInstanceId);
            _flippedStatesByInstance.Remove(_callingInstanceId);
            
            if (GameManager.Instance != null && _callingNpcObject != null)
            {
                GameManager.Instance.SetNpcInteractionProcessed(_callingNpcObject, true);
            }
        }
        _selectedCardInstance = null;
        isEnhanceCompleted = false;
        
        base.Close();
    }

}
