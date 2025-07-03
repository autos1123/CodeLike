using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class UIManager:MonoSingleton<UIManager>
{
    private Dictionary<string, UIBase> _uiInstances = new Dictionary<string, UIBase>();
    private List<GameObject> uiPrefabs;

    [SerializeField] private string currentSceneName;

    protected override bool Persistent => false;

    protected override void Awake()
    {
        base.Awake();
        uiPrefabs = new List<GameObject>();
        InitializeUI();
        
    }

    /// <summary>
    /// 테스트용 후추 삭제
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("f1");
            ShowUI<DestinyBoard>();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("f2");
            Hide<DestinyBoard>();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("인벤열림");
            ShowUI<InventoryUI>();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("인벤닫힘");
            Hide<InventoryUI>();
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
    }

    public void ToggleOptionUI<T>() where T : UIBase
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
}
