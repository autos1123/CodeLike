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
    VFX
}
public interface IPoolObject
{
    public GameObject GameObject { get; }
    public PoolType PoolType { get; }
    public int PoolSize { get; }
}
