using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    TableManager tableManager;
    public TableManager TableManager
    {
        get { return tableManager; }
        protected set {  tableManager = value; }
    }
    SaveManager saveManager;
    public SaveManager SaveManager
    {
        get { return saveManager; }
        protected set {  saveManager = value; }
    }
    protected override void Awake()
    {
        base.Awake();

        TableManager = TableManager.Instance;
        SaveManager = SaveManager.Instance;

        tableManager.Initialization();
        saveManager.Initialization();
    }
}
