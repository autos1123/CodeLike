using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoolingHPBar : MonoBehaviour, IPoolObject
{
    [Header("풀링 설정")]
    [SerializeField] private PoolType poolType = PoolType.hpBar;
    [SerializeField] private int poolSize = 20;
    private Camera mainCam;

    [Header("체력 UI")]
    [SerializeField] private Image hpBarImage;

    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.forward = mainCam.transform.forward;
    }
    void LateUpdate()
    {
        transform.LookAt(mainCam.transform);
    }

    public void HpBarUpdate(float hpRatio)
    {
        hpBarImage.fillAmount = Mathf.Clamp01(hpRatio);
    }
}
