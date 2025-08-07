using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    /// <summary>
    /// 리스트의 요소를 주어진 System.Random 인스턴스로 섞는다.
    /// </summary>
    /// <typeparam name="T">리스트 요소의 타입</typeparam>
    /// <param name="list">셔플할 리스트</param>
    /// <param name="rng">시드를 설정한 System.Random 인스턴스</param>
    public static void Shuffle<T>(this List<T> list, System.Random rng)
    {
        if(list == null || list.Count <= 1) return;
        int n = list.Count;
        while(n > 0)
        {
            int k = rng.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public static List<T> ShuffleWithSeed<T>(this IEnumerable<T> source, int seed)
    {
        System.Random rng = new System.Random(seed);
        return source.OrderBy(_ => rng.Next()).ToList();
    }
}

public static class StackExtension
{
    public static bool RemoveItem<T>(this Stack<T> stack , T value)
    {
        if(stack == null || stack.Count == 0)
            return false;

        var temp = new Stack<T>();
        bool removed = false;

        // 요소 꺼내며 찾기
        while(stack.Count > 0)
        {
            var item = stack.Pop();
            if(!removed && EqualityComparer<T>.Default.Equals(item, value))
            {
                removed = true; // 해당 항목 제거
                continue;
            }
            temp.Push(item);
        }

        // 원래대로 복구
        while(temp.Count > 0)
        {
            stack.Push(temp.Pop());
        }

        return removed;
    }
}

