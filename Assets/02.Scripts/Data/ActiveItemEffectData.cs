using UnityEngine;

[System.Serializable]
public class ActiveItemEffectData
{
    public int SkillID;
    public string Name;
    public SkillType Type;
    public float Power;
    public float Range;
    public float Duration;
    public float Cooldown;
    public CostType CostType;
    public float Cost;
    public string EffectPrefab;
    public string Description;
}
