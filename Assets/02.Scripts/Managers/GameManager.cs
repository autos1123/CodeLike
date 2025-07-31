using System;
using System.Collections;
using System.Collections.Generic;
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

    public Dictionary<int, bool> processedNpcObjects = new();

    public float Gold=0;
    public Dictionary<ConditionType, Dictionary<ModifierType, float>> ConditionModifier;
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
        
        if (processedNpcObjects == null)
        {
            processedNpcObjects = new Dictionary<int, bool>();
        }
    }
    private void Start()
    {
        //Application.targetFrameRate = 60;
    }
    // 각종 NPC 상호작용의 완료 상태를 설정하는 공용 메소드
    public void SetNpcInteractionProcessed(GameObject npcObject, bool processed)
    {
        int instanceId = npcObject.GetInstanceID();
        processedNpcObjects[instanceId] = processed;
    }

    // 각종 NPC 상호작용의 완료 상태를 확인하는 공용 메소드
    public bool GetNpcInteractionProcessed(GameObject npcObject)
    {
        int instanceId = npcObject.GetInstanceID();
        return processedNpcObjects.TryGetValue(instanceId, out bool processed) && processed;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(_player != null)
        {
            Destroy(_player);
            _player = null;
        }
        _player = GameObject.FindGameObjectWithTag(TagName.Player);

        StartCoroutine(DelayedSceneInit());

    }
    IEnumerator DelayedSceneInit()
    {
        yield return new WaitForSecondsRealtime(1f);


        if(SceneManager.GetActiveScene().name.CompareTo("PrototypeScene") == 0)
        {
            if(ConditionModifier != null)
            {
                _player.GetComponent<BaseController>().Condition.SetModifier(ConditionModifier);
                _player.GetComponent<BaseController>().Condition.CurrentConditions[ConditionType.Gold] = 0;
                _player.GetComponent<BaseController>().Condition.ChangeGold(Gold);
            }
        }
        if(SceneManager.GetActiveScene().name.CompareTo("LobbyScene") == 0)
        {
            _player.GetComponent<BaseController>().Condition.ResetModifier();
            _player.GetComponent<BaseController>().Condition.ChangeGold(-Gold * 0.2f);
        }
    }
}
