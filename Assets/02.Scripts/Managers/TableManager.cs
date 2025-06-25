using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class TableManager : MonoSingleton<TableManager>
{
    [SerializeField] List<ScriptableObject> tableList = new List<ScriptableObject>();

    private Dictionary<Type, ITable> tableDic = new Dictionary<Type, ITable>();
    protected override void Awake()
    {
        base.Awake();

        foreach (var tableObj in tableList)
        {
            if (tableObj is ITable table)
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
