using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseTable<T> : ScriptableObject, ITable where T : class 
{
    public List<T> dataList = new List<T>();  
    public Dictionary<int, T> DataDic { get; protected set; } = new Dictionary<int, T>();
    public Type Type { get; private set; }

    public virtual void CreateTable()
    {
        Type = GetType();
    }

    public virtual T GetDataByID(int id)
    {
        if (DataDic.TryGetValue(id, out T value))
            return value;

        Debug.LogError($"ID {id}를 찾을 수 없습니다.");
        return default(T);
    }
}

