using System.Collections;
using UnityEngine;

public class TutorialDummy : MonoBehaviour, IDamagable
{
    [SerializeField] private float hp = 100;
    private float curHp;
    [SerializeField] private float dropGold = 0;
    [SerializeField] private GameObject dropPassiveItemBox;
    [SerializeField] private GameObject dropActiveItemBox;

    private Animator anim;
    private PlayerController player;
    private GameObject hpBar;
    public PoolingHPBar HpUI => hpBar.GetComponent<PoolingHPBar>();

    private void Start()
    {
        curHp = hp;
        anim = GetComponentInChildren<Animator>();
        player = FindObjectOfType<PlayerController>();
        StartCoroutine(WaitForDataLoad());
    }

    private void Initialize()
    {
        // 체력 UI 초기화
        hpBar = PoolManager.Instance.GetObject(PoolType.hpBar);
        hpBar.transform.SetParent(transform);
        hpBar.transform.localPosition = Vector3.zero + Vector3.up * 2f; // HP Bar 위치 조정
        HpBarUpdate();
    }

    public void HpBarUpdate()
    {
        HpUI.HpBarUpdate(curHp/hp);
    }

    public bool GetDamaged(float damage)
    {
        if(curHp <= 0) return false;

        curHp -= damage;
        HpBarUpdate();

        if(curHp <= 0)
        {
            player.Condition.ChangeGold(dropGold);
            anim.SetTrigger("Die");
            Invoke(nameof(Die), 1f);

            return false;
        }

        anim.SetTrigger("Hit");
        SoundManager.Instance.PlaySFX(GameManager.Instance.Player.transform.position,"HitSound");
        return true;
    }

    private void Die()
    {
        Vector3 dropPosition_1 = transform.position + Vector3.up * 0.5f;
        
        Instantiate(dropPassiveItemBox, dropPosition_1, Quaternion.identity);
        
        gameObject.SetActive(false);
        GameEvents.TriggerMonsterKilled();
    }

    protected virtual IEnumerator WaitForDataLoad()
    {
        yield return new WaitUntil(() => PoolManager.Instance.IsInitialized);
        Initialize();
    }
}
