using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DamageType
{
    Normal,
    Critical
}

public class PoolingDamageUI : MonoBehaviour, IPoolObject
{
    [SerializeField] private PoolType poolType = PoolType.DamageUI;
    [SerializeField] private int poolSize = 20;
    private Camera mainCam;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color criticalColor;

    [SerializeField] private float fadeDuration;
    [SerializeField] private float moveDistance;
    private Vector3 startPosition;
    private float timer = 0f;

    public GameObject GameObject => gameObject;
    public PoolType PoolType => poolType;
    public int PoolSize => poolSize;

    private void OnEnable()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= fadeDuration)
        {
            PoolManager.Instance.ReturnObject(this);
            return;
        }

        transform.position = Vector3.Lerp(startPosition, startPosition + Vector3.up * moveDistance, timer / fadeDuration);
        SetTextAlpha(Mathf.Lerp(1f, 0f, timer / fadeDuration));
    }

    void LateUpdate()
    {
        transform.forward = mainCam.transform.forward;
    }

    public void InitDamageText(Vector3 startPos ,DamageType damageType, float damage)
    {
        startPosition = startPos;
        transform.position = startPosition;

        damageText.text = damage.ToString("F0"); // 소수점 없이 정수로 표시

        switch(damageType)
        {
            case DamageType.Normal:
                damageText.color = normalColor;
                break;
            case DamageType.Critical:
                damageText.color = criticalColor;
                break;
        }

        timer = 0f;
        SetTextAlpha(1);
    }

    private void SetTextAlpha(float alpha)
    {
        Color textColor = damageText.color;
        textColor.a = alpha;
        damageText.color = textColor;
    }
}
