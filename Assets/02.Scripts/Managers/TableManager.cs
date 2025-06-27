using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class TableManager:MonoSingleton<TableManager>
{
    GameManager _gameManager;

    [SerializeField] List<ScriptableObject> tableList = new List<ScriptableObject>();

    private Dictionary<Type, ITable> tableDic = new Dictionary<Type, ITable>();

    private AsyncOperationHandle<ScriptableObject> _loadHandle;

    public bool loadComplete { get; private set; } = false;

    /// <summary>
    /// 초기화
    /// </summary>
    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
        LoadTablesAsync();
    }
    /// <summary>
    /// 라벨을 통해 어드레서블에 올린 데이터 탐색하여 저장
    /// </summary>
    private void LoadTablesAsync()
    {
        Addressables.LoadAssetsAsync<ScriptableObject>(
            AddressbleLabels.TableLabel,
            (table) =>
            {
                tableList.Add(table);
            }
        ).Completed += (handle) =>
        {
            foreach(var tableObj in tableList)
            {
                if(tableObj is ITable table)
                {
                    table.CreateTable();
                    tableDic[table.Type] = table;
                }
            }
            loadComplete = true;  // 테이블 다 로드하고 등록한 다음에 true로 바꿈
            Debug.Log("[TableManager] 테이블 로드 및 등록 완료");
        };
    }

    public T GetTable<T>() where T : class
    {
        return tableDic[typeof(T)] as T;
    }

}
