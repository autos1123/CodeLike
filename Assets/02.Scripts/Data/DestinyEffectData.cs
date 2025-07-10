using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class DestinyEffectData
{
    public int ID;
    public string Name;
    public EffectType effectType;
    public EffectedTarget effectedTarget;
    public ConditionType conditionType;
    public float value;
    public string dsecription;
}
