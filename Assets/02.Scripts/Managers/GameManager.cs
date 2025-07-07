using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Play,
    ViewChange,
    Stop,
}
[DefaultExecutionOrder(-100)]
public class GameManager : MonoSingleton<GameManager>
{
    //스테이지 마다 생성할 맵의 수
    public int[] stageMapCountData = {0, 5, 6, 7, 8, 9, 10 };

    public DestinyData curDestinyData;// 현재 적용중인 운명

    public event Action<DestinyData> onDestinyChange;

    [SerializeField]private GameObject _player;
    public GameObject Player
    {
        get { return _player; }
    }

    public GameState curGameState;
    public event Action onGameStateChange;

    public void setCurDestinyData(DestinyData destinyData)
    {
        this.curDestinyData = destinyData;
        onDestinyChange?.Invoke(curDestinyData);
    }
    public void setState(GameState gameState)
    {
        curGameState = gameState;
        onGameStateChange?.Invoke();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        onDestinyChange += HandleDestinyChange;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        onDestinyChange -= HandleDestinyChange;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(_player != null)
        {
            Destroy(_player);
            _player=null;
        }
        _player = GameObject.FindGameObjectWithTag(TagName.Player);
    }

    void HandleDestinyChange(DestinyData data)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);


        if(positiveEffect.affectedTarget == AffectedTarget.Map)
        {
            stageMapCountData = stageMapCountData.Select(n => n + (int)positiveEffect.value).ToArray();
        }

        if(negativeEffect.affectedTarget == AffectedTarget.Map)
        {
            stageMapCountData = stageMapCountData.Select(n => n - (int)negativeEffect.value).ToArray();
        }

    }
}
