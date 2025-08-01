using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;


public class SoundManager:MonoSingleton<SoundManager>
{
    [SerializeField] AudioMixer audioMixer;

    Dictionary<string, AudioClip> bgmdic;
    Dictionary<string, AudioClip> sfxdic;
    //플레이하는 AudioSource
    [SerializeField] SoundSource audioBgm;

    bool isBGMLoad = false;
    bool isSFXLoad = false;
    public void Start()
    {
        Init();
    }
    public void Init()
    {
        Addressables.LoadAssetAsync<AudioMixer>("MainMixer")
            .Completed += (handle) =>
            {
                audioMixer = handle.Result;
            };
        LoadBGMAsync();
        LoadSFXAsync();
    }

    /// <summary>
    /// 어드레서블에 등록된 BGM , SFX불러오기
    /// </summary>
    private void LoadBGMAsync()
    {
        bgmdic = new Dictionary<string, AudioClip>();

        Addressables.LoadAssetsAsync<AudioClip>(
            AddressbleLabels.BGMLabel,
            (source) =>
            {
                if(!bgmdic.ContainsKey(source.name))
                {
                    bgmdic[source.name] = source;
                }
            }
        ).Completed += (handle) => { isBGMLoad = true; };
    }
    private void LoadSFXAsync()
    {
        sfxdic = new Dictionary<string, AudioClip>();
        Addressables.LoadAssetsAsync<AudioClip>(
            AddressbleLabels.SFXLabel,
            (source) =>
            {
                if(!sfxdic.ContainsKey(source.name))
                {
                    sfxdic[source.name] = source;
                }
            }
        ).Completed += (handle) => { isSFXLoad = true; };
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos">소리 생성 포지션</param>
    /// <param name="key">재생할 소리 이름</param>
    public void PlaySFX(Vector3 pos, string key)
    {
        var sound = PoolManager.Instance.GetObject(PoolType.SoundSource);
        sound.transform.position = GetVector3(pos);
        SoundSource soundSource = sound.GetComponent<SoundSource>();
        if(sfxdic.TryGetValue(key, out var clip))
        {
            soundSource.Play(clip, true);
        }
        else
        {
            Debug.Log("그런 소리 없다");
        }
    }

    public void PlaySFX(float time, Vector3 pos, string key)
    {
        var sound = PoolManager.Instance.GetObject(PoolType.SoundSource);
        sound.transform.position = GetVector3(pos);
        SoundSource soundSource = sound.GetComponent<SoundSource>();

        if(sfxdic.TryGetValue(key, out var clip))
        {
            soundSource.Play(time, clip, true);
        }
        else
        {
            Debug.Log("그런 소리 없다");
        }
    }
    public void PlayBGM(Transform transform, string key)
    {
        if(audioBgm != null && !audioBgm.Equals(null)) StopBGM();

        var sound = PoolManager.Instance.GetObject(PoolType.SoundSource);
        sound.transform.position = transform != null ? transform.position : Vector3.zero;
        sound.transform.SetParent(null);

        var follower = sound.GetComponent<FollowTarget>();
        if(follower == null)
            follower = sound.AddComponent<FollowTarget>();

        follower.target = transform;

        SoundSource soundSource = sound.GetComponent<SoundSource>();

        if(bgmdic.TryGetValue(key, out var clip))
        {
            soundSource.Play(clip, false);
        }
        else
        {
            Debug.Log("그런 소리 없다");
        }
        audioBgm = soundSource;
    }

    Vector3 GetVector3(Vector3 vector3)
    {
        return new Vector3(vector3.x, vector3.y, vector3.z);
    }
    // 현재 재생 중인 배경 음악 정지
    public void StopBGM()
    {
        if(audioBgm != null && !audioBgm.Equals(null))
        {
            audioBgm.Stop();
        }
        audioBgm = null;
    }

    public bool IsOnLoad()
    {
        return isBGMLoad && isSFXLoad;
    }
}
