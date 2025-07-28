using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BossPattern : ScriptableObject
{
    protected BaseCondition _BaseCondition;
    protected CombatController _CombatController;

    [SerializeField] protected float damageCoefficient;
    [SerializeField] protected AnimationClip animationClip;

    public AnimationClip AnimationClip => animationClip;

    public void Init(BaseCondition baseCondition, CombatController combatController)
    {
        _BaseCondition = baseCondition;
        _CombatController = combatController;
    }

    public abstract void PatternAction();
}
