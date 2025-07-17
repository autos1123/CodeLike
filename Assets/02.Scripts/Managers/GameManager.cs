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

    DestinyManager destinyManager;

    [SerializeField]private GameObject _player;
    public GameObject Player
    {
        get { return _player; }
    }

    public GameState curGameState;
    public event Action onGameStateChange;


    public void setState(GameState gameState)
    {
        curGameState = gameState;
        onGameStateChange?.Invoke();
    }

    void OnEnable()
    {
        if(destinyManager == null) 
            destinyManager = DestinyManager.Instance;

        SceneManager.sceneLoaded += OnSceneLoaded;
        destinyManager.onDestinyChange += HandleDestinyChange;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        destinyManager.onDestinyChange -= HandleDestinyChange;
        destinyManager = null;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
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

    void HandleDestinyChange(DestinyData data, int i)
    {
        DestinyEffectData positiveEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.PositiveEffectDataID);
        DestinyEffectData negativeEffect = TableManager.Instance.GetTable<DestinyEffectDataTable>().GetDataByID(data.NegativeEffectDataID);

        if(positiveEffect.effectedTarget == EffectedTarget.Map)
        {
            stageMapCountData = stageMapCountData.Select(n => n + (int)positiveEffect.value * i).ToArray();
        }

        if(negativeEffect.effectedTarget == EffectedTarget.Map)
        {
            stageMapCountData = stageMapCountData.Select(n => n - (int)negativeEffect.value * i).ToArray();
        }

    }
}
