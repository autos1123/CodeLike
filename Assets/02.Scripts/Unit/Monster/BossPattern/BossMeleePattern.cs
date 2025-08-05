using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossMeleePattern", menuName = "ScriptableObjects/BossPattern/BossMeleePattern", order = 1)]
public class BossMeleePattern : BossPattern
{
    float attackPower;
    float criticalChance;
    float criticalDamage;

    public BossMeleePattern()
    {
        
    }

    public override void PatternAction()
    {
        Collider[] hitColliders = _CombatController.GetTargetColliders(LayerMask.GetMask("Player"));

        attackPower = _BaseCondition.GetTotalCurrentValue(ConditionType.AttackPower);
        criticalChance = _BaseCondition.GetTotalCurrentValue(ConditionType.CriticalChance);
        criticalDamage = _BaseCondition.GetTotalCurrentValue(ConditionType.CriticalDamage);

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable player))
            {
                bool isCritical = Random.value < criticalChance;

                float finalDamage = attackPower;
                if(isCritical)
                {
                    finalDamage *= criticalDamage;
                }

                DamageType damageType = isCritical ? DamageType.Critical : DamageType.Normal;
                PoolingDamageUI damageUI = PoolManager.Instance.GetObject(PoolType.DamageUI).GetComponent<PoolingDamageUI>();
                damageUI.InitDamageText(player.GetDamagedPos(), damageType, finalDamage);

                // 플레이어에게 피해를 입히는 로직
                player.GetDamaged(finalDamage);
            }
        }
    }
}
