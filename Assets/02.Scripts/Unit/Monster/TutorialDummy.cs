using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummy : MonoBehaviour, IDamagable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public bool GetDamaged(float damage)
    {
        anim.SetTrigger("Hit");
        return true;
    }
}
