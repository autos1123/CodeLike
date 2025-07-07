using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ModifierType
{
    Upgrade = 0,
    BuffEnhance,
    ItemEnhance
}
public class BaseCondition
{
    [SerializeField]protected ConditionData data;
    public ConditionData Data => data;

    public Dictionary<ConditionType, float> CurrentConditions { get; private set; }
    public Dictionary<ConditionType, Dictionary<ModifierType, float>> CondifionModifier { get; private set; }

    public Dictionary<ConditionType, Action> statModifiers = new();
    public BaseCondition(ConditionData data)
    {
        this.data = data;
        CurrentConditions = data.GetCurrentConditions();
        CondifionModifier = new Dictionary<ConditionType, Dictionary<ModifierType, float>>();

        foreach(var item in CurrentConditions)
        {
            statModifiers[item.Key] = null;
        }
    }

    /// <summary>
    /// 특정 컨디션 타입에 대한 현재 값을 반환합니다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetValue(ConditionType type)
    {
        if(CurrentConditions.TryGetValue(type, out float value))
            return value;
        Debug.LogError($"ConditionType {type}를 찾을 수 없습니다.");
        return 0f;
    }

    /// <summary>
    /// 특정 컨디션의 수정값을 반환합니다.
    /// </summary>
    /// <param name="m_Type">수정자 타입</param>
    /// <param name="c_Type">컨디션 타입</param>
    /// <returns></returns>
    public float GetModifierValue(ModifierType m_Type, ConditionType c_Type)
    {
        if(CondifionModifier.TryGetValue(c_Type, out Dictionary<ModifierType, float> modifierDict))
        {
            if(modifierDict.TryGetValue(m_Type, out float value))
            {
                return value;
            }
            Debug.LogError($"ModifierType {m_Type}를 찾을 수 없습니다.");
        }
        else
        {
            Debug.LogError($"ConditionType {c_Type}에 대한 Modifier가 존재하지 않습니다.");
        }

        return 0f;
    }
    /// <summary>
    /// 컨디션 전체 증가치 반환
    /// </summary>
    /// <param name="c_Type"></param>
    /// <returns></returns>
    public float GetModifierValue(ConditionType c_Type)
    {
        if(CondifionModifier.TryGetValue(c_Type, out Dictionary<ModifierType, float> modifierDict))
        {
            return CondifionModifier[c_Type].Values.Sum();
        }
        else
        {
            Debug.LogError($"ConditionType {c_Type}에 대한 Modifier가 존재하지 않습니다.");
        }

        return 0f;
    }

    /// <summary>
    /// 컨디션을 문자열로 변환하여 반환
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetStatus(ConditionType type)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(CurrentConditions[type]);
        if(CondifionModifier.ContainsKey(type))
        {
            sb.Append('(');
            sb.Append($"{CondifionModifier[type].Values.Sum()}");
            sb.Append(')');
        }
        return sb.ToString();
    }
    

    /// <summary>
    /// 강화 타입에 따른 특정 컨디션 값의 증가치 변화(증가, 감소)
    /// </summary>
    /// <param name="c_type">적용될 컨디션 타입</param>
    /// <param name="m_type">강화 타입</param>
    /// <param name="value"></param>
    public void ChangeModifierValue(ConditionType c_type, ModifierType m_type, float value)
    {
        if(CondifionModifier.TryGetValue(c_type, out Dictionary<ModifierType, float> modifierDict))
        {
            if(modifierDict.TryGetValue(m_type, out float currentValue))
            {
                modifierDict[m_type] += value;
            }
            else
            {
                modifierDict[m_type] = value;
            }       
        }
        else
        {
            CondifionModifier[c_type] = new Dictionary<ModifierType, float> { { m_type, value } };
        }
        
        statModifiers[c_type]?.Invoke();
    }

    /// <summary>
    /// 컨디션 증가치 초기화
    /// 게임이 끝나는 시점에 호출
    /// </summary>
    public void ResetModifier()
    {
        CondifionModifier.Clear();
    }

    public bool GetDamaged(float damage)
    {
        if(!CurrentConditions.ContainsKey(ConditionType.HP))
        {
            Debug.LogError("HP ConditionType이 존재하지 않습니다.");
            return false;
        }

        CurrentConditions[ConditionType.HP] -= damage;
        if(CurrentConditions[ConditionType.HP] <= 0)
        {
            CurrentConditions[ConditionType.HP] = 0;
            return true; // 사망 처리
        }

        return false; // 사망하지 않음
    }
}
