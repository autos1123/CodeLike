using UnityEngine;

public enum PoolType
{
    none,
    temp,
    projectile,
    PlayerProjectile,
    hpBar,
    SoundSource,
    AoE,
    Zone,
    Heal,
    ArrowProjectile
}
public interface IPoolObject
{
    public GameObject GameObject { get; }
    public PoolType PoolType { get; }
    public int PoolSize { get; }
}
