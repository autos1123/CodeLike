using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoolingHPBar : MonoBehaviour, IPoolObject
{
    [Header("풀링 설정")]
    [SerializeField] private PoolType poolType = PoolType.hpBar;
    [SerializeField] private int poolSize = 20;
    private Camera mainCam;

    [Header("체력 UI")]
    [SerializeField] private Image hpBarImage;

    public GameObject GameObject => this != null ? gameObject : null;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    private void OnEnable()
    {
        if(mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if(mainCam != null)
        {
            transform.forward = mainCam.transform.forward;
        }
    }

    public void HpBarUpdate(float hpRatio)
    {
        hpBarImage.fillAmount = Mathf.Clamp01(hpRatio);
    }
}
