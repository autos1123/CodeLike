using System.Collections;
using UnityEngine;

public class SoundSource : MonoBehaviour ,IPoolObject
{
    public AudioSource audioSource;
    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize;
    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip,bool issfx)
    {
        audioSource.clip = clip;
        audioSource.Play();
        if(issfx) { StartCoroutine(wiatEnd(clip.length)); }
    }
    public void Stop()
    {
        audioSource.Stop();
        returnPool();
    }

    IEnumerator wiatEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        returnPool();
    }
    public void returnPool()
    {
        PoolManager.Instance.ReturnObject(this);
    }
}
