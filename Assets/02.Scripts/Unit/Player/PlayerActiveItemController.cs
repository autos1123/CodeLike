using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActiveItemController : MonoBehaviour
{
    private Dictionary<SkillType, ISkillExecutor> executors;

    private void Start()
    {
        executors = new Dictionary<SkillType, ISkillExecutor>
        {
            { SkillType.Projectile, new ProjectileSkillExecutor() },

        };
    }
}
