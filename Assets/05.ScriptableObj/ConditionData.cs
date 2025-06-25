using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType
{
    None = 0,
    HP,
    Stamina,
    StaminaRegen,
    AttackPower,
    Defense,
    AttackSpeed,
    MoveSpeed,
    JumpPower,
    CriticalChance,
    CriticalDamage
}

[Serializable]
public class ConditionEntry
{
    [SerializeField] private ConditionType type;
    [SerializeField] private float value;

    public ConditionEntry(ConditionType type, float value) 
    {
        this.type = type;
        this.value = value;
    }

    public ConditionType Type => type;
    public float Value => value;

    public void SetValue(float newValue)
    {
        value = newValue;
    }
}

[CreateAssetMenu(fileName = "ConditionData", menuName = "ScriptableObjects/ConditionData")]
public class ConditionData : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] private Dictionary<ConditionType, ConditionEntry> conditions = new Dictionary<ConditionType, ConditionEntry>();
    public string CharacterName => characterName;
    public Dictionary<ConditionType, ConditionEntry> Conditions => conditions;

    /// <summary>
    /// 컨디션 딕셔너리를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Dictionary<ConditionType, float> GetCurrentConditions()
    {
        Dictionary<ConditionType, float> currentConditions = new Dictionary<ConditionType, float>();
        foreach(var condition in conditions)
        {
            currentConditions[condition.Key] = condition.Value.Value;
        }
        return currentConditions;
    }

    /// <summary>
    /// 특정 컨디션 타입에 대한 ConditionEntry를 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public ConditionEntry GetCondition(ConditionType type)
    {
        if(conditions.TryGetValue(type, out ConditionEntry entry))
        {
            return entry;
        }
        else
        {
            Debug.LogWarning($"Condition {type} not found.");
            return null;
        }
    }

    /// <summary>
    /// 컨디션 종류 추가
    /// 데이터 로드 과정에서 사용됨
    /// </summary>
    /// <param name="type">추가할 컨디션</param>
    /// <param name="value">값</param>
    public void AddCondition(ConditionType type, float value)
    {
        if(conditions.ContainsKey(type))
        {
            Debug.LogWarning($"Condition {type} already exists. Updating value.");
            conditions[type] = new ConditionEntry(type, value);
        }
        else
        {
            conditions.Add(type, new ConditionEntry(type, value));
        }
    }

    /// <summary>
    /// 컨디션 초기화
    /// 데이터 로드 과정에서 사용됨
    /// </summary>
    /// <param name="type">초기화할 컨디션</param>
    /// <param name="value"></param>
    public void InitCondition(ConditionType type, float value)
    {
        if(conditions.ContainsKey(type))
        {
            conditions[type].SetValue(value);
        }
        else
        {
            Debug.LogWarning($"Condition {type} does not exist. Adding new condition.");
            AddCondition(type, value);
        }
    }
}
