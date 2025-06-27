using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType
{
    // 공통 컨디션 타입
    HP = 0,
    Stamina,
    StaminaRegen,
    AttackPower,
    AttackSpeed,
    AttackRange,
    Defense,
    MoveSpeed,
    JumpPower,
    CriticalChance,
    CriticalDamage,
    // Enemy 전용 컨디션 타입
    PatrolRange,
    ChaseRange
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

[System.Serializable]
public class ConditionData
{
    [SerializeField] private int id;
    [SerializeField] private string characterName;
    [SerializeField] private List<ConditionEntry> initialConditions = new();
    private Dictionary<ConditionType, ConditionEntry> conditions = new Dictionary<ConditionType, ConditionEntry>();
    public int ID { get { return id; } set { id = value; } }
    public string CharacterName { get { return characterName; } set { characterName = value; } }
    public Dictionary<ConditionType, ConditionEntry> Conditions => conditions;

    public void InitDictionary()
    {
        // 초기 컨디션을 딕셔너리에 추가
        foreach(var entry in initialConditions)
        {
            conditions[entry.Type] = entry;
        }
    }

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
    /// 특정 컨디션 타입에 대한 값을 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool TryGetCondition(ConditionType type, out float value)
    {
        if(conditions.TryGetValue(type, out ConditionEntry entry))
        {
            value = entry.Value;
            return true;
        }
        else
        {
            Debug.LogWarning($"Condition {type} not found.");
            value = 0f;
            return false;
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
        initialConditions.Add(conditions[type]);
    }
}
