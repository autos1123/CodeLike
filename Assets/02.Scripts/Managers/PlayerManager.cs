using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 정보 저장 매니저
/// </summary>
public class PlayerManager : MonoSingleton<PlayerManager>
{
    private GameObject _player;
    protected override bool Persistent => false;
    public GameObject Player
    {
        get { return _player; }
    }
    protected override void Awake()
    {
        base.Awake();

        _player = GameObject.FindGameObjectWithTag(TagName.Player);
    }
}
