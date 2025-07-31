using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookObstacle :ObstacleController, IDamagable
{
    [SerializeField] private float health = 100f;
    private float currentHealth;

    [SerializeField] private GameObject shieldEffect;
    [SerializeField] private EnemyController[] enemies;
    public bool isShieldActive { get; private set; } = false;

    [SerializeField] private ParticleSystem destroyEffect;
    [SerializeField] private ParticleSystem patternEffect;

    [SerializeField] private Room room;

    [Header("Obstacle Controllers")]
    [SerializeField] private ObstacleController[] obstacleControllers;
    private ObstacleController currentPattern;
    [SerializeField] private float patternInterval = 5f;

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

    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(WaitForNextPattern());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
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

    private void PlayRandomPattern()
    {
        if(obstacleControllers.Length == 0)
            return;
        int randomIndex = Random.Range(0, obstacleControllers.Length);
        currentPattern = obstacleControllers[randomIndex];
        obstacleControllers[randomIndex].PatternPlay();
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

    public override void PatternPlay()
    {
        
    }

    IEnumerator WaitForNextPattern()
    {
        while(true)
        {
            yield return new WaitForSeconds(patternInterval);
            patternEffect.Clear();
            patternEffect.Play();
            yield return new WaitUntil(() => patternEffect.isPlaying == false);
            PlayRandomPattern();
            yield return new WaitUntil(() => currentPattern.isPatternEnd == true);
        }
    }
}
