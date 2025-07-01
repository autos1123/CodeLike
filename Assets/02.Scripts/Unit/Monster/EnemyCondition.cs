using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCondition:BaseCondition
{
    public EnemyCondition(ConditionData data) : base(data)
    {
    }

    public float GetCurrentHpRatio()
    {
        if(!Data.TryGetCondition(ConditionType.HP, out float max))
        {
            Debug.LogError("ConditionType.HP를 찾을 수 없습니다.");
            return 0;
        }

        return GetValue(ConditionType.HP) / max;
    }
}
