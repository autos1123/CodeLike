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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
}
