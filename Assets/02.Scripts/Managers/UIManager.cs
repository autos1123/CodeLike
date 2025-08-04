using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum UILabel
{
    None,
    IntroUI,
    InGameUI,
    TutorialUI
}
public class UIManager:MonoSingleton<UIManager>
{
    private Dictionary<string, UIBase> _uiInstances = new Dictionary<string, UIBase>();
    private List<GameObject> uiPrefabs;
    private bool uiLoaded = false;
    private List<Action> _onUILoaded = new();

    [SerializeField] private List<UILabel> uiLabel;
    private ContextualUIHint _currentContextualHintUI;

    public bool IsInitialized { get; private set; } = false;
    protected override bool Persistent => false;

    
    protected override void Awake()
    {
        base.Awake();
        uiPrefabs = new List<GameObject>();
        InitializeUI();
    }
    private IEnumerator Start()
    {
        // Addressables 로딩이 끝나고
        yield return new WaitUntil(() => StageManager.Instance.CurrentStage != null);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            ToggleUI<EnhanceBoard>();
        }
    }
    private void InitializeUI()
    {
        List<string> labels = new List<string>();
        foreach (var label in uiLabel)
        {
            if (label != UILabel.None)
            {
                labels.Add(label.ToString());
            }
        }
        
        Addressables.LoadAssetsAsync<GameObject>(
            labels,
            (GameObject) =>
            {
                var ui = Instantiate(GameObject, this.transform );
                uiPrefabs.Add(ui);
                ui.SetActive(false); // 초기에는 비활성화
            },
            UnityEngine.AddressableAssets.Addressables.MergeMode.Union // 여러 라벨의 에셋을 모두 합쳐 로드하도록 지정
        ).Completed += (handle) =>
        {
            foreach(var tableObj in uiPrefabs)
            {
                if(tableObj.TryGetComponent<UIBase>(out var uIBase))
                {
                    _uiInstances[uIBase.UIName] = uIBase;
                }
                // if (tableObj.name.Contains("TooltipUI"))
                // {
                //     tooltipManager.RegisterTooltipUI(tableObj);
                // }
            }
            uiLoaded = true;
            
            //콜백 실행
            foreach (var callback in _onUILoaded)
            {
                callback?.Invoke();
            }
            _onUILoaded.Clear();

            Addressables.Release(handle);
        };
    }

    public void ToggleUI<T>() where T : UIBase
    {
        if(GetUI<T>().gameObject.activeSelf)
            Hide<T>();
        else
            ShowUI<T>();
    }

    public void ShowUI<T>() where T : UIBase
    {
        if(_uiInstances.TryGetValue(typeof(T).Name , out var ui))
        {
            ui.Open();
        }
    }

    public void Hide<T>() where T : UIBase
    {
        if(_uiInstances.TryGetValue(typeof(T).Name, out var ui))
        {
            ui.Close();
        }
    }

    public T GetUI<T>() where T : UIBase
    {
        if(!_uiInstances.ContainsKey(typeof(T).Name))
            return null;
        return _uiInstances[typeof(T).Name] as T;
    }

    public bool TryGetUI<T>(out T result) where T : UIBase
    {
        if(_uiInstances.TryGetValue(typeof(T).Name, out var ui))
        {
            result = ui as T;
            return true;
        }

        result = null;
        return false;
    }
    // 특정 힌트 메시지를 가진 ContextualUIHint를 표시하는 전용 메서드 추가
    public void ShowContextualHint(string message)
    {
        if (_currentContextualHintUI == null) // 아직 참조가 없다면 딕셔너리에서 가져옴
        {
            if (TryGetUI<ContextualUIHint>(out var hintUI))
            {
                _currentContextualHintUI = hintUI;
            }
            else
            {
                Debug.LogError("[UIManager] ContextualUIHint가 로드되지 않았거나 찾을 수 없습니다.");
                return;
            }
        }
        
        // 새로운 힌트를 표시하기 전에 현재 표시 중인 힌트가 있다면 강제로 닫음
        if (_currentContextualHintUI.gameObject.activeSelf)
        {
            _currentContextualHintUI.Close();
        }

        _currentContextualHintUI.SetHintText(message); 
        _currentContextualHintUI.Open();
        
        _currentContextualHintUI.transform.SetAsLastSibling();
    }

    // ContextualUIHint를 숨기는 전용 메서드 추가
    public void HideContextualHint()
    {
        if (TryGetUI<ContextualUIHint>(out var hintUI))
        {
            hintUI.Close();
        }
    }
    public void ShowConfirmPopup(
        string message, 
        Action onConfirm, 
        Action onCancel = null,string confirmText = "예", 
        string cancelText = "아니오",bool playOpenSound = true
        )
    {
        if (_uiInstances.TryGetValue(nameof(ConfirmPopup), out var ui))
        {
            var popup = ui as ConfirmPopup;
            popup.Setup(message, onConfirm, onCancel,confirmText, cancelText);
            if(playOpenSound)
            {
                popup.Open();
            }
            else
            {
                popup.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("[UIManager] ConfirmPopup이 로드되지 않았습니다.");
        }
    }
    public void OnAllUIReady(Action callback)
    {
        if(uiLoaded) callback?.Invoke();
        else _onUILoaded.Add(callback);
    }
    // UIManager의 로딩 상태를 외부에 노출하는 메서드 추가
    public bool IsUILoaded()
    {
        return uiLoaded;
    }


}
