using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestinyManager : MonoSingleton<DestinyManager>
{
    TableManager _tableManager;
    List<DestinyData> _destinyDatas;
    DestinyBoard _destinyBoard;
    protected override bool Persistent => false;
    protected override void OnDestroy()
    {
        if(Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        _destinyDatas = tableManager.GetTable<DestinyDataTable>().dataList;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public List<DestinyData> GetDestinys(int count)
    {
        return _destinyDatas.ShuffleData().Take(count).ToList();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {        
        var board = GameObject.FindObjectOfType<DestinyBoard>();

        if(board != null)
        {
            _destinyBoard = board;
        }
        else
        {
            Debug.LogWarning("보드를 씬에서 찾을 수 없음");
        }
    }

    public void OpenBoard()
    {
        _destinyBoard.gameObject.SetActive(true);
    }

    public void CloseBoard()
    {
        _destinyBoard.gameObject.SetActive(false);
    }
}
