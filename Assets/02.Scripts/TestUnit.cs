using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : MonoBehaviour, IDamagable
{
    public bool GetDamaged(float damage)
    {
        return true;
    }
}
