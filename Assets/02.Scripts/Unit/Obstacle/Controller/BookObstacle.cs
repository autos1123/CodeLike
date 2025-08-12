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
    private float patternTimer = 0f;

    [Header("HPUI")]
    [SerializeField] private GameObject hpBar;

    private Coroutine hintTextCoroutine;

    public bool GetDamaged(float damage)
    {
        if(isShieldActive)
            return false; // 방어막이 활성화된 경우 데미지를 받지 않음

        currentHealth -= Mathf.Clamp(damage, 0f, currentHealth);
        HpBarUpdate();
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
        currentHealth = health;

        // 체력 UI 초기화
        hpBar = PoolManager.Instance.GetObject(PoolType.hpBar);
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.zero + Vector3.up * 2f; // HP Bar 위치 조정
        HpBarUpdate();

        StartCoroutine(WaitForNextPattern());
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if(PoolManager.HasInstance && hpBar != null)
            PoolManager.Instance.ReturnObject(hpBar.GetComponent<IPoolObject>());

        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        CheckShieldActive();
        shieldEffect.SetActive(isShieldActive);
    }

    public void HpBarUpdate()
    {
        hpBar.GetComponent<PoolingHPBar>().HpBarUpdate(currentHealth / health);
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
        StopAllCoroutines();
        UIManager.Instance.Hide<ContextualUIHint>();
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
            yield return null;

            if(isPlaying)
            {
                patternTimer += Time.deltaTime;
            }

            if(patternTimer >= patternInterval)
            {
                if(hintTextCoroutine != null)
                {
                    StopCoroutine(hintTextCoroutine);
                }
                hintTextCoroutine = StartCoroutine(HintFadeOut());

                patternEffect.Clear();
                patternEffect.Play();
                yield return new WaitUntil(() => patternEffect.isPlaying == false);
                PlayRandomPattern();
                yield return new WaitUntil(() => currentPattern.isPatternEnd == true);
                patternTimer = 0f;
            }
        }
    }

    IEnumerator HintFadeOut()
    {
        ContextualUIHint hint = UIManager.Instance.GetUI<ContextualUIHint>();
        if(hint != null)
        {
            hint.SetHintText("창조주의 마도서가 스킬을 사용합니다. 주의하세요! \nV키를 눌러 시점을 변환하여 회피하세요.");
        }

        UIManager.Instance.ShowUI<ContextualUIHint>();
        yield return new WaitForSeconds(2f);
        UIManager.Instance.Hide<ContextualUIHint>();

        hintTextCoroutine = null;
    }

    public Vector3 GetDamagedPos()
    {
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            return col.bounds.center + (col.transform.up * (col.bounds.size.y / 2)); // 충돌체의 중심 위치 반환
        }
        return transform.position;
    }
}
