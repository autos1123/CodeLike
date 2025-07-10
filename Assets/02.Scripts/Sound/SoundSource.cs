using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSource : MonoBehaviour ,IPoolObject
{
    AudioSource audioSource;
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] private PoolType poolType;
    [SerializeField] private int poolSize;
    public GameObject GameObject => gameObject;

    public PoolType PoolType => poolType;

    public int PoolSize => poolSize;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(float delaeTime, AudioClip clip, bool issfx)
    {
        StartCoroutine(DelayRoutine(delaeTime, clip, issfx));
    }


    public void Play(AudioClip clip,bool issfx)
    {
        if(issfx) audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        else audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];

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
        PoolManager.Instance.ReturnObject(this);
    }
}
