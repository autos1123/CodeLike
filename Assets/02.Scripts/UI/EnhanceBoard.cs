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
    private int _callingNpcId;
    
    private Dictionary<int, List<EnhanceData>> _npcEnhanceDataCache = new();
    private Dictionary<int, Dictionary<EnhanceCard, bool>> _npcCardFlippedStates = new();

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
        Open(0);
    }
    public void Open(int callingNpcId)
    {
        base.Open();
        _callingNpcId = callingNpcId;
        
        if(_tableManager == null) _tableManager = TableManager.Instance;
        
        if (!_npcEnhanceDataCache.TryGetValue(callingNpcId, out var npcEnhanceList))
        {
            npcEnhanceList = _tableManager.GetTable<EnhanceDataTable>().dataList
                .ShuffleWithSeed(callingNpcId) // NPC ID를 시드로 활용
                .Take(cards.Length)
                .ToList();

            _npcEnhanceDataCache.Add(callingNpcId, npcEnhanceList);
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

        // 카드 뒤집힘 상태 복원
        if (_npcCardFlippedStates.TryGetValue(_callingNpcId, out var flippedDict))
        {
            foreach (var card in cards)
            {
                if (flippedDict.TryGetValue(card, out bool isFlipped))
                    card.SetFlippedState(isFlipped);
                else
                    card.SetFlippedState(false);
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
        if (!_npcCardFlippedStates.ContainsKey(_callingNpcId))
            _npcCardFlippedStates[_callingNpcId] = new();

        _npcCardFlippedStates[_callingNpcId][card] = isFlipped;
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
        if (_selectedCardInstance == null)
        {
            foreach(var item in cards)
            {
                item.gameObject.SetActive(false); 
                item.SetInteractable(false); 
                item.SetSelectButtonActive(false);
            }
        }
        else // 카드를 선택하여 닫혔을 경우 (강화가 완료된 경우)
        {
            foreach (var card in cards)
                card.Clear();
            
            _selectedCardInstance = null;

            // 캐시 초기화
            _npcEnhanceDataCache.Remove(_callingNpcId);
            _npcCardFlippedStates.Remove(_callingNpcId);
            
            if (GameManager.Instance != null && _callingNpcId != 0)
            {
                GameManager.Instance.SetEnhancementProcessed(_callingNpcId, true);
            }
        }
        base.Close();
    }

}
