using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 가변데이터 저장용
/// </summary>
public class SaveManager : MonoSingleton<SaveManager>
{
    GameManager _gameManager;
    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
}
