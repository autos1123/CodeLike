using UnityEngine;


public abstract class Obstacle : MonoBehaviour
{
    protected ObstacleController controller;
    protected Rigidbody rb;

    protected bool isPlaying = true; // 게임이 플레이 중인지 여부

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        GameManager.Instance.onGameStateChange += OnGameStateChange;
    }

    protected virtual void OnDisable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.onGameStateChange -= OnGameStateChange;
    }

    public void Init(ObstacleController controller)
    {
        this.controller = controller;
    }

    /// <summary>
    /// 게임 상태가 변경될 때 호출되는 메서드입니다.
    /// </summary>
    protected void OnGameStateChange()
    {
        if(GameManager.Instance.curGameState == GameState.Play)
        {
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
        }

        SetPlayMode();
    }

    protected abstract void SetPlayMode();
}
