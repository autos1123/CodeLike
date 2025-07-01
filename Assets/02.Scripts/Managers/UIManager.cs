using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager:MonoSingleton<UIManager>
{
    private Dictionary<string, UIBase> _uiInstances = new Dictionary<string, UIBase>();
    private List<GameObject> uiPrefabs;
    protected override void Awake()
    {
        base.Awake();
        InitializeUI();
    }
    private void InitializeUI()
    {
        Addressables.LoadAssetsAsync<GameObject>(
            AddressbleLabels.UIabel,
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
                    _uiInstances[uIBase.name] = uIBase;
                }
            }            
            Debug.Log("[TableManager] 테이블 로드 및 등록 완료");
        };
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
        return _uiInstances[name] as T;
    }
}
