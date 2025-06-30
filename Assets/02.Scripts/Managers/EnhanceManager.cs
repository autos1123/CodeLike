using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnhanceManager :MonoSingleton<EnhanceManager>
{
    TableManager _tableManager;
    List<EnhanceData> _enhanceDatas;
    EnhanceBoard _enhanceBoard;
    protected override bool Persistent => false;

    protected override void OnDestroy()
    {
        if(Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Init(TableManager tableManager)
    {
        _tableManager = tableManager;
        _enhanceDatas = _tableManager.GetTable<EnhanceDataTable>().dataList;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    public List<EnhanceData> GetEnhance(int count)
    {
        return _enhanceDatas.ShuffleData().Take(count).ToList();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var board = GameObject.FindObjectOfType<EnhanceBoard>();

        if(board != null)
        {
            _enhanceBoard = board;
        }
        else
        {
            Debug.LogWarning("보드를 씬에서 찾을 수 없음");
        }
    }

    public void OpenBoard()
    {
        _enhanceBoard.gameObject.SetActive(true);
    }

    public void CloseBoard()
    {
        _enhanceBoard.gameObject.SetActive(false);
    }

}
