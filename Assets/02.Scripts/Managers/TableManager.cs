using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-100)]
public class TableManager:MonoSingleton<TableManager>
{

    [SerializeField] List<ScriptableObject> tableList;

    [SerializeField] private Dictionary<Type, ITable> tableDic;

    private void Start()
    {
        if(tableDic == null) tableDic = new();
        foreach(var tableObj in tableList)
        {
            if(tableObj is ITable table)
            {
                table.CreateTable();
                tableDic[table.Type] = table;
            }
        }
    }
#if UNITY_EDITOR
    public void LoadTables()
    {
        if (tableList == null) tableList = new();
        

        tableList.Clear();
        

        // "Assets/Data/Tables" 경로 안의 ScriptableObject 검색
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets/05.ScriptableObj/SO" });

        foreach(string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if(asset == null)
            {
                Debug.LogWarning($"[TableManager] null asset at {path}");
                continue;
            }

            if(asset is ITable itable) tableList.Add(asset);
        }

    }

    public void ClearTables()
    {
        tableList.Clear();
    }

#endif
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"> T 테이블 타입</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        return tableDic[typeof(T)] as T;
    }

}
