using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObstacleController : MonoBehaviour
{
    [SerializeField] protected float damage;
    protected bool isPlaying = true; // 게임이 플레이 중인지 여부
    public bool isPatternEnd { get; protected set; } = false; // 패턴이 끝났는지 여부

    protected virtual void OnEnable()
    {
        GameManager.Instance.onGameStateChange += OnGameStateChange;
    }

    protected virtual void OnDisable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.onGameStateChange -= OnGameStateChange;
    }

    public abstract void PatternPlay();

    public void Attack(IDamagable player)
    {
        PoolingDamageUI damageUI = PoolManager.Instance.GetObject(PoolType.DamageUI).GetComponent<PoolingDamageUI>();
        damageUI.InitDamageText(player.GetDamagedPos(), DamageType.Normal, damage);

        player.GetDamaged(damage);
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
    }
}
