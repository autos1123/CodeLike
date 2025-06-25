using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TableManager:MonoSingleton<TableManager>
{
    [SerializeField] List<ScriptableObject> tableList = new List<ScriptableObject>();

    private Dictionary<Type, ITable> tableDic = new Dictionary<Type, ITable>();

    private AsyncOperationHandle<ScriptableObject> _loadHandle;



    protected override void Awake()
    {        
        base.Awake();

        LoadTablesAsync();
        foreach(var tableObj in tableList)
        {
            if(tableObj is ITable table)
            {
                table.CreateTable();
                tableDic[table.Type] = table;
            }
        }
    }
    /// <summary>
    /// 라벨을 통해 어드레서블에 올린 데이터 탐색하여 저장
    /// </summary>
    private void LoadTablesAsync()
    {
        Addressables.LoadAssetsAsync<ScriptableObject>(
            TableAddressble.TableLabel,
            (table) =>
            {
                tableList.Add(table);
            }
        );
    }

    public T GetTable<T>() where T : class
    {
        return tableDic[typeof(T)] as T;
    }

}
