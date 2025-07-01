using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoultionData
{
    public List<Resolution> resolutions { get; private set; } = new List<Resolution>(); // 해상도 리스트

    public ResoultionData()
    {
        InitResolution(); // 생성자에서 해상도 리스트 초기화
    }

    /// <summary>
    /// 해상도 리스트 저장 및 현재 해상도 탐색
    /// </summary>
    public void InitResolution()
    {
        // 16:9 비율 해상도만 추가
        for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            float max = GCD(Screen.resolutions[i].width, Screen.resolutions[i].height); // 해상도의 최대공약수 계산
            if((Screen.resolutions[i].width / max == 16) && (Screen.resolutions[i].height / max == 9)) // 해상도가 16:9인 해상도만 리스트에 추가
                resolutions.Add(Screen.resolutions[i]);
        }
    }

    /// <summary>
    /// 최대 공약수
    /// </summary>
    /// <param name="a">값1</param>
    /// <param name="b">값2</param>
    /// <returns>값1과 값2의 최대 공약수</returns>
    int GCD(int a, int b) { if(b == 0) { return a; } else { return GCD(b, a % b); } }
}
