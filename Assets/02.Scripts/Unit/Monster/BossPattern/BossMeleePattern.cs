using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossMeleePattern", menuName = "ScriptableObjects/BossPattern/BossMeleePattern", order = 1)]
public class BossMeleePattern : BossPattern
{
    public override void PatternAction()
    {
        Collider[] hitColliders = _CombatController.GetTargetColliders(LayerMask.GetMask("Player"));

        foreach(var hitCollider in hitColliders)
        {
            if(hitCollider.TryGetComponent(out IDamagable player))
            {
                // 플레이어에게 피해를 입히는 로직
                player.GetDamaged(_BaseCondition.GetTotalCurrentValue(ConditionType.AttackPower));
            }
        }
    }
}
