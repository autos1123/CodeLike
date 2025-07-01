using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoSingleton<GameManager>
{
    TableManager _tableManager;
    SaveManager _saveManager;
    SoundManager _soundManager;

    public TableManager TableManager
    {
        get { return _tableManager; }
    }
    public SaveManager SaveManager
    {
        get { return _saveManager; }
    }
    public SoundManager SoundManager
    {
        get { return _soundManager; }
    }

    protected override void Awake()
    {
        base.Awake();

        _tableManager = TableManager.Instance;
        _saveManager = SaveManager.Instance;
        //_soundManager = SoundManager.Instance;

        _tableManager.Init(Instance);
        _saveManager.Init(Instance);
        //_soundManager.Init();
    }
}
