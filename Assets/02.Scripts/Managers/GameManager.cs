using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    TableManager _tableManager;
    public TableManager TableManager
    {
        get { return _tableManager; }
        protected set {  _tableManager = value; }
    }
    SaveManager _saveManager;
    public SaveManager SaveManager
    {
        get { return _saveManager; }
        protected set {  _saveManager = value; }
    }
    protected override void Awake()
    {
        base.Awake();

        TableManager = TableManager.Instance;
        SaveManager = SaveManager.Instance;

        _tableManager.Init(Instance);
        _saveManager.Init(Instance);
    }
}
