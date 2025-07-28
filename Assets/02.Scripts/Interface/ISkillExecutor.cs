using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillExecutor
{
    void Execute(ActiveItemEffectData data, Transform caster);
}
