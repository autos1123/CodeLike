using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSource : MonoBehaviour ,IPoolObject
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize;
    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    public void Play(float delaeTime, AudioClip clip, bool issfx)
    {
        StartCoroutine(DelayRoutine(delaeTime, clip, issfx));
    }
    
    public void Play(AudioClip clip,bool issfx)
    {
        ViewManager.Instance.OnViewChanged += HandleViewModeChange;

        HandleViewModeChange(ViewManager.Instance.CurrentViewMode);

        if(issfx) 
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        else 
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];

        audioSource.clip = clip;
        audioSource.Play();
        if(issfx) { StartCoroutine(wiatEnd(clip.length)); }
        else audioSource.loop = true;
    }
    private void OnDestroy()
    {
        Stop();
    }
    public void Stop()
    {
        audioSource.Stop();
        if(ViewManager.Instance == null) return;
        ViewManager.Instance.OnViewChanged -= HandleViewModeChange;
        returnPool();
    }

    IEnumerator wiatEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.clip = null;
        returnPool();
    }

    IEnumerator DelayRoutine(float delaeTime, AudioClip clip, bool issfx)
    {
        yield return new WaitForSeconds(delaeTime);
        Play(clip, issfx);
    }

    public void returnPool()
    {
        if(gameObject == null || this == null) return;
        if(PoolManager.HasInstance)
            PoolManager.Instance.ReturnObject(this);
    }

    private void HandleViewModeChange(ViewModeType viewModeType)
    {
        audioSource.spatialBlend = (viewModeType == ViewModeType.View2D) ? 0f : 1f;
    }
}
