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
    protected ConditionData data;
    public ConditionData Data => data;

    public Dictionary<ConditionType, float> CurrentConditions { get; private set; }
    public Dictionary<ConditionType, Dictionary<ModifierType, float>> ConditionModifier { get; private set; }

    public Dictionary<ConditionType, Action> statModifiers = new();

    public bool IsDied => GetValue(ConditionType.HP) <= 0;

    public BaseCondition(ConditionData data)
    {
        this.data = data;
        CurrentConditions = data.GetCurrentConditions();
        ConditionModifier = new Dictionary<ConditionType, Dictionary<ModifierType, float>>();        

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
        float curValue = 0;
        float modifierValue = 0;

        if(CurrentConditions.TryGetValue(type, out curValue))
        {
            if(ConditionModifier.ContainsKey(type))
                modifierValue = ConditionModifier[type].Values.Sum();

            return curValue + modifierValue;
        }

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
        if(ConditionModifier.TryGetValue(c_Type, out Dictionary<ModifierType, float> modifierDict))
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
        if(ConditionModifier.TryGetValue(c_Type, out Dictionary<ModifierType, float> modifierDict))
        {
            return ConditionModifier[c_Type].Values.Sum();
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
        if(ConditionModifier.ContainsKey(type))
        {
            sb.Append('(');
            sb.Append($"{ConditionModifier[type].Values.Sum()}");
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
        if(ConditionModifier.TryGetValue(c_type, out Dictionary<ModifierType, float> modifierDict))
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
            ConditionModifier[c_type] = new Dictionary<ModifierType, float> { { m_type, value } };
        }
        
        statModifiers[c_type]?.Invoke();
    }

    /// <summary>
    /// 컨디션 증가치 초기화
    /// 게임이 끝나는 시점에 호출
    /// </summary>
    public void ResetModifier()
    {
        ConditionModifier.Clear();
    }

    public bool GetDamaged(float damage)
    {
        if(!CurrentConditions.ContainsKey(ConditionType.HP))
        {
            Debug.LogError("HP ConditionType이 존재하지 않습니다.");
            return false;
        }

        CurrentConditions[ConditionType.HP] -= damage;
        statModifiers[ConditionType.HP]?.Invoke(); // 체력 변경 이벤트

        if(CurrentConditions[ConditionType.HP] <= 0)
        {
            CurrentConditions[ConditionType.HP] = 0;
            return true; // 사망 처리
        }

        return false; // 사망하지 않음
    }

    public void Heal(float Heal)
    {
        if(!CurrentConditions.ContainsKey(ConditionType.HP))
        {
            Debug.LogError("HP ConditionType이 존재하지 않습니다.");
            return;
        }

        CurrentConditions[ConditionType.HP] += Heal;
        CurrentConditions[ConditionType.HP] = Mathf.Min(CurrentConditions[ConditionType.HP], GetValue(ConditionType.HP));
        statModifiers[ConditionType.HP]?.Invoke(); // 체력 변경 이벤트
    }

    /// <summary>
    /// 특정 컨디션 비율을 반환합니다. (0.0f ~ 1.0f)
    /// </summary>
    /// <returns></returns>
    public float GetConditionRatio(ConditionType type)
    {
        if(!Data.TryGetCondition(type, out float max))
        {
            Debug.LogError("ConditionType.HP를 찾을 수 없습니다.");
            return 0;
        }

        return GetValue(type) / max;
    }

    public void ChangeGold(float value)
    {
        if(!CurrentConditions.ContainsKey(ConditionType.Gold))
        {
            Debug.LogError("Gold ConditionType이 존재하지 않습니다.");
            return;
        }

        CurrentConditions[ConditionType.Gold] += value;
        statModifiers[ConditionType.Gold]?.Invoke(); // 골드 변경 이벤트
    }
}
