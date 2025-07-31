using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookObstacle : MonoBehaviour, IDamagable
{
    [SerializeField] private float health = 100f;
    private float currentHealth;

    [SerializeField] private GameObject shieldEffect;
    [SerializeField] private EnemyController[] enemies;
    public bool isShieldActive { get; private set; } = false;

    [SerializeField] private ParticleSystem destroyEffect;

    [SerializeField] private Room room;

    public bool GetDamaged(float damage)
    {
        if(isShieldActive)
            return true; // 방어막이 활성화된 경우 데미지를 받지 않음

        currentHealth -= Mathf.Clamp(damage, 0f, currentHealth);
        if(currentHealth <= 0f)
        {
            room.CheckClear();
            Die();
            return false; 
        }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        CheckShieldActive();
        shieldEffect.SetActive(isShieldActive);
    }

    private void CheckShieldActive()
    {
        if(enemies.Length > 0)
        {
            isShieldActive = false;

            foreach(EnemyController enemy in enemies)
            {
                if(enemy.Condition == null)
                    continue;

                if(!enemy.Condition.IsDied)
                {
                    isShieldActive = true;
                    break;
                }
            }
        }
    }

    private void Die()
    {
        destroyEffect.Play();
        Invoke(nameof(Destroy), 0.2f);
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }
}
