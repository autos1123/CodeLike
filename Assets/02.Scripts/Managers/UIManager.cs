using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager:MonoSingleton<UIManager>
{
    private Dictionary<string, UIBase> _uiInstances = new Dictionary<string, UIBase>();
    private List<GameObject> uiPrefabs;
    private bool uiLoaded = false;
    private List<Action> _onUILoaded = new();

    [SerializeField] private string currentSceneName;

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
        yield return new WaitUntil(() => StageManager.Instance.currentStage != null);

        BuildMinimap(StageManager.Instance.currentStage);
    }

    /// <summary>
    /// 테스트용 후추 삭제
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("스테이터스");
            ToggleUI<StatusBoard>();
        }
    }
    private void InitializeUI()
    {
        string uiLabel = currentSceneName.Equals("TitleScene") ? AddressbleLabels.TitleUILabel : AddressbleLabels.InGameUILabel;

        Addressables.LoadAssetsAsync<GameObject>(
            uiLabel,
            (GameObject) =>
            {
                var ui = Instantiate(GameObject, this.transform);
                uiPrefabs.Add(ui);        
            }
        ).Completed += (handle) =>
        {
            foreach(var tableObj in uiPrefabs)
            {
                if(tableObj.TryGetComponent<UIBase>(out var uIBase))
                {
                    _uiInstances[uIBase.UIName] = uIBase;
                }
                if (tableObj.name.Contains("TooltipUI"))
                {
                    TooltipManager.Instance.RegisterTooltipUI(tableObj);
                }
            }
            Debug.Log("[TableManager] 테이블 로드 및 등록 완료");            
        };
        Debug.Log("[UIManager] 등록된 UI 목록:");
        foreach(var kvp in _uiInstances)
        {
            Debug.Log($"- {kvp.Key}");
        }
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

    public void ShowConfirmPopup(string message, Action onConfirm, Action onCancel = null,string confirmText = "예", string cancelText = "아니오")
    {
        if (_uiInstances.TryGetValue(nameof(ConfirmPopup), out var ui))
        {
            var popup = ui as ConfirmPopup;
            popup.Setup(message, onConfirm, onCancel,confirmText, cancelText);
            popup.Open();
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

    public void BuildMinimap(StageData stageData)
    {
        if(TryGetUI<MinimapUI>(out var minimap))
        {
            var minimapData = MinimapBuilder.BuildFromStage(stageData, stageData.connections);
            minimap.GenerateMinimap(minimapData);
            Debug.Log($" Minimap 생성 완료: {minimapData.Count}개 방");
        }
        else
        {
            Debug.LogError(" MinimapUI를 찾을 수 없습니다.");
        }
    }
}
