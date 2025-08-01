using DG.Tweening;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EnhanceCard:MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _enhaceDescription;
    
    [SerializeField] private Image _cardImage; // 카드의 이미지를 표시할 UI Image 컴포넌트
    public Button _selectButton; // 선택 버튼 (처음엔 숨김)
    [SerializeField] private GameObject _frontContent; // 카드의 앞면 내용 (제목, 설명)을 담는 GameObject
    
    private bool _isFlipped = false; // 카드가 뒤집혔는지 여부
    private EnhanceBoard _enhanceBoard;
    private EnhanceData _enhanceData;
    
    private float _randomIncreaseValue = 0f;
    
    [SerializeField] private float _flipSpeed = 0.05f; // 각 프레임당 보여줄 시간 (초)
    [SerializeField] private float _secondClickAnimDuration = 0.15f; // 두 번째 클릭 애니메이션 총 시간
    [SerializeField] private float _secondClickShrinkFactor = 0.9f; // 두 번째 클릭 시 얼마나 작아질지


    void Start()
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(Click);
    }

    public void init(EnhanceData enhanceData, EnhanceBoard enhanceBoard)
    {
        _enhanceBoard = enhanceBoard; // 참조 저장
        _enhanceData = enhanceData;
        
        float value = Random.Range(enhanceData.minvalue, enhanceData.maxvalue);

        // 1. 플레이어 기본 ConditionData에서 "초기값" 가져오기
        var playerConditionData = TableManager.Instance.GetTable<ConditionDataTable>().dataList
            .FirstOrDefault(cd => cd.CharacterName == "Player");
        float baseStat = 0f;
        if(playerConditionData != null)
        {
            playerConditionData.InitConditionDictionary();
            playerConditionData.TryGetCondition(_enhanceData.ConditionType, out baseStat);
        }

        // 2. 플레이어 현재 Condition 값 가져오기
        float currentStat = 0f;
        if(GameManager.Instance.Player.TryGetComponent<PlayerController>(out var playerController))
        {
            currentStat = playerController.Condition.GetTotalCurrentValue(_enhanceData.ConditionType);
        }

        // 3. 증가량/설명
        float increaseStat = baseStat * value; // 기준은 "초기값" 배율 (원하는대로 바꿔도 됨)
        _randomIncreaseValue = increaseStat;
        
        // 초기 상태는 뒷면 (숨김)
        _isFlipped = false;
        _selectButton.gameObject.SetActive(false); 
        _selectButton.onClick.RemoveAllListeners();
        _selectButton.onClick.AddListener(SelectCard);

        gameObject.SetActive(true);
        _button.interactable = true;
        
        RefreshUI();
    }
    void Click()
    {
        if (!_button.interactable) return;
        
        if (!_isFlipped)
        { 
            SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"CardFlip");
            StartCoroutine(FlipAnimation());  // 아직 안 뒤집었으면 뒤집기
        }
        else
        {
            SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"CardSecondClick");
            _selectButton.gameObject.SetActive(true);
            StartCoroutine(SecondClickAnimation());
            _enhanceBoard.CardSelected(this);
        }
    }
    
    // public void FlipCard()
    // {
    //     if (_isFlipped) return;
    //
    //     _isFlipped = true;
    //     _cardImage.sprite = _enhanceData.flipFrontFrames.FirstOrDefault();
    //     _frontContent.SetActive(true); 
    //     
    //     // 애니메이션 효과 (나중에 추가)
    // }
    private IEnumerator FlipAnimation()
    {
        if (_isFlipped) yield break; 
        
        _enhanceBoard.exitButton.gameObject.SetActive(false);
        _button.interactable = false; // 애니메이션 시작 시 클릭 비활성화
        
        if (_enhanceData.flipBackFrames != null)
        {
            for (int i = 0; i < _enhanceData.flipBackFrames.Count; i++) 
            {
                _cardImage.sprite = _enhanceData.flipBackFrames[i];
                yield return new WaitForSeconds(_flipSpeed);
            }
        }
        _frontContent.SetActive(true); // 앞면 내용 활성화

        if (_enhanceData.flipFrontFrames != null)
        {
            for (int i = 0; i < _enhanceData.flipFrontFrames.Count; i++)
            {
                _cardImage.sprite = _enhanceData.flipFrontFrames[i];
                yield return new WaitForSeconds(_flipSpeed);
            }
        }
        _isFlipped = true; 
        
        _enhanceBoard.CardFlipped(this, _isFlipped);
        
        _enhanceBoard.exitButton.gameObject.SetActive(true);
        _button.interactable = true; // 애니메이션 종료 후 클릭 다시 활성화
        
    }
    private IEnumerator SecondClickAnimation()
    {
        Transform cardTransform = _cardImage.rectTransform;
        Vector3 originalScale = cardTransform.localScale;

        // 작아지는 애니메이션
        cardTransform.DOScale(originalScale * _secondClickShrinkFactor, _secondClickAnimDuration / 2)
            .SetEase(Ease.OutQuad); 
        yield return new WaitForSeconds(_secondClickAnimDuration / 2); 

        // 커지는 애니메이션
        cardTransform.DOScale(originalScale, _secondClickAnimDuration / 2)
            .SetEase(Ease.InQuad); 
        yield return new WaitForSeconds(_secondClickAnimDuration / 2); 

    }

    void SelectCard()
    {
        StartCoroutine(CoroutineSelectCardEffect());
    }
    private IEnumerator CoroutineSelectCardEffect() // 선택 버튼이 눌렸을 때 호출될 메소드
    {
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"CardSelect");
        yield return StartCoroutine(PlayCardBumpAnimation());
        yield return new WaitForSeconds(0.5f);
        
        if (GameManager.Instance.Player.TryGetComponent<PlayerController>(out var playerController))
        {
            playerController.Condition.ChangeModifierValue(
                _enhanceData.ConditionType, 
                ModifierType.BuffEnhance, 
                _randomIncreaseValue
                );
        }
        _enhanceBoard.isEnhanceCompleted = true;
        _enhanceBoard.Close(); 
    }
    
    private IEnumerator PlayCardBumpAnimation()
    {
        RectTransform cardRect = _cardImage.rectTransform;
        Vector3 originalPos = cardRect.anchoredPosition;

        float moveAmount = 50f; // 위로 올라갈 거리
        float moveDuration = 0.3f;

        // 위로 올라가기
        cardRect.DOAnchorPosY(originalPos.y + moveAmount, moveDuration).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(moveDuration);
        
        // 다시 내려오기
        cardRect.DOAnchorPosY(originalPos.y, moveDuration).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(moveDuration);
    }
    
    public void SetSelectButtonActive(bool active)
    {
        _selectButton.gameObject.SetActive(active);
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }
    // UI (텍스트, 이미지)를 현재 _enhanceData와 _isFlipped 상태에 맞춰 갱신하는 메소드
    public void RefreshUI() 
    {
        if (_enhanceData == null) // 데이터가 없으면 UI를 비움
        {
            _title.text = string.Empty;
            _enhaceDescription.text = string.Empty;
            _cardImage.sprite = null;
            _frontContent.SetActive(false);
            return;
        }
        
        _title.text = _enhanceData.name;

        // 설명 텍스트 재구성 (init에서 계산된 _randomIncreaseValue 사용)
        string statName = _enhanceData.name.Replace("증가", "");
        float currentStat = 0f; // 현재 스탯 값은 Refresh 시에도 다시 가져올 수 있음
        if (GameManager.Instance.Player.TryGetComponent<PlayerController>(out var playerController))
        {
            currentStat = playerController.Condition.GetTotalMaxValue(_enhanceData.ConditionType);
        }
        string line1 = $"{statName}이 {_randomIncreaseValue:F1} 증가합니다.";
        string line2 = $"강화 후: {currentStat:F2} → {currentStat + _randomIncreaseValue:F1}";
        _enhaceDescription.text = line1 + "\n" + line2;

        // 이미지와 앞면 내용 갱신
        if (_isFlipped)
        {
            // 뒤집힌 상태면 앞면 프레임의 마지막을 보여줌
            if (_enhanceData.flipFrontFrames != null && _enhanceData.flipFrontFrames.Any())
            {
                _cardImage.sprite = _enhanceData.flipFrontFrames.LastOrDefault(); // 마지막 앞면 프레임
            }
            else
            {
                _cardImage.sprite = null; // 프레임이 없으면 이미지 비움
            }
            _frontContent.SetActive(true);
        }
        else
        {
            // 뒤집히지 않은 상태면 뒷면 프레임의 첫 번째를 보여줌
            if (_enhanceData.flipBackFrames != null && _enhanceData.flipBackFrames.Any())
            {
                _cardImage.sprite = _enhanceData.flipBackFrames.FirstOrDefault(); // 첫 번째 뒷면 프레임
            }
            else
            {
                _cardImage.sprite = null; // 프레임이 없으면 이미지 비움
            }
            _frontContent.SetActive(false);
        }
    }
    public void SetFlippedState(bool flipped)
    {
        _isFlipped = flipped;
        RefreshUI();
    }
    public void Clear()
    {
        _isFlipped = false; // 기본적으로 뒤집히지 않은 상태로 클리어
        
        _frontContent.SetActive(false);
        _selectButton.gameObject.SetActive(false);

        // 클리어 시에는 카드를 비활성화 (EnhanceBoard.Open()에서 다시 활성화되도록)
        gameObject.SetActive(false);
        _button.interactable = false; // 버튼 비활성화

        // Clear 후에는 기본 상태 (뒷면)를 강제로 적용
        SetFlippedState(false);

    }
}
