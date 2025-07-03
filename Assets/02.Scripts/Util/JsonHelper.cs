using System;
using UnityEngine;

[Serializable]
public class Wrapper<T>
{
    public T[] datas;
}

public static class JsonHelper
{
    // 배열 → JSON
    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        var wrapper = new Wrapper<T> { datas = array };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    // JSON → 배열
    public static T[] FromJson<T>(string json)
    {
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.datas;
    }
}