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
