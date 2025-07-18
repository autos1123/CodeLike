using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummy : MonoBehaviour, IDamagable
{
    [SerializeField] private float hp = 100;
    [SerializeField] private float dropGold = 10;
    [SerializeField] private GameObject dropItemBox;

    private Animator anim;
    private PlayerController player;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerController>();
    }

    public bool GetDamaged(float damage)
    {
        if(hp <= 0) return false;

        hp -= damage;
        if(hp <= 0)
        {
            player.Condition.ChangeGold(dropGold);
            anim.SetTrigger("Die");
            Invoke(nameof(Die), 1f);

            return false;
        }

        anim.SetTrigger("Hit");
        return true;
    }

    private void Die()
    {
        Instantiate(dropItemBox, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
