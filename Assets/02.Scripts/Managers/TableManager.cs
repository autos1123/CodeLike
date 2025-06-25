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

    public void LoadTablesAsync()//튜터님에게 질문
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup(TableAddressble.TableGroup);

        if(group == null)
        {
            Debug.LogError($"Group '{TableAddressble.TableGroup}' not found!");
            return;
        }

        foreach(var entry in group.entries)
        {
            Addressables.LoadAssetAsync<ScriptableObject>(entry.address).Completed +=
                (AsyncOperationHandle<ScriptableObject> AsyncOperationHandle) =>
                {
                    _loadHandle = AsyncOperationHandle;        
                    if(_loadHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        tableList.Add(_loadHandle.Result);
                    }

                };
        }
    }

    protected override void Awake()
    {
        base.Awake();

        foreach(var tableObj in tableList)
        {
            if(tableObj is ITable table)
            {
                table.CreateTable();
                tableDic[table.Type] = table;
            }
        }
    }

    public T GetTable<T>() where T : class
    {
        return tableDic[typeof(T)] as T;
    }

}
