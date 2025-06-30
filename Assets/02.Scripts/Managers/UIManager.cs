using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIManager:MonoSingleton<UIManager>
{
    private Dictionary<string, UIBase> _uiInstances = new Dictionary<string, UIBase>();
    [SerializeField]private List<GameObject> uiPrefabs;
    private void Awake()
    {
        InitializeUI();
    }
    private void InitializeUI()
    {
        Addressables.LoadAssetsAsync<UIBase>(
            AddressbleLabels.UIabel,
            (uiBase) =>
            {
                var ui =  Instantiate(uiBase.gameObject, this.transform);
                uiPrefabs.Add(ui);
                uiBase.Close();                
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
}
