using System;
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

    public event Action onDestinyChange;

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
        onDestinyChange?.Invoke();
    }
    public void setState(GameState gameState)
    {
        curGameState = gameState;
        onGameStateChange?.Invoke();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
}
