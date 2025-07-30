using System;
using System.Collections.Generic;
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

    public Dictionary<int, bool> processedEnhancerObjects = new();

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
    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;
        
        if (processedEnhancerObjects == null)
        {
            processedEnhancerObjects = new Dictionary<int, bool>();
        }
    }
    private void Start()
    {
        //Application.targetFrameRate = 60;
    }
    // 강화 NPC의 완료 상태를 설정하는 공용 메소드
    public void SetEnhancementProcessed(GameObject npcObject, bool processed)
    {
        int instanceId = npcObject.GetInstanceID();
        processedEnhancerObjects[instanceId] = processed;
    }

    // 강화 NPC의 완료 상태를 확인하는 공용 메소드
    public bool GetEnhancementProcessed(GameObject npcObject)
    {
        int instanceId = npcObject.GetInstanceID();
        return processedEnhancerObjects.TryGetValue(instanceId, out bool processed) && processed;
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
